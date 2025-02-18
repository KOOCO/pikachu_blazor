using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Identity;
using Kooco.Pikachu.PikachuAccounts.ExternalUsers;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Data;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Kooco.Pikachu.PikachuAccounts;

[RemoteService(IsEnabled = false)]
public class PikachuAccountAppService(IConfiguration configuration, IdentityUserManager identityUserManager,
    IIdentityRoleRepository identityRoleRepository, IMemoryCache memoryCache, IEmailSender emailSender,
    IMyIdentityUserRepository identityUserRepository, IExternalUserAppService externalUserAppService,
    ITenantSettingsAppService tenantSettingsAppService) : PikachuAppService, IPikachuAccountAppService
{
    public const string VerificationCodePrefix = "__VerificationCode:";
    public const string PasswordResetCodePrefix = "__ResetPasswordCode:";
    public MemoryCacheEntryOptions CacheEntryOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
    };

    public async Task<GenericResponseDto> FindByTokenAsync(LoginMethod method, string thirdPartyToken)
    {
        if (method == LoginMethod.UserNameOrPassword)
        {
            return new GenericResponseDto(false, "Invalid_Method");
        }
        var externalUser = await SetupExternalUserAsync(method, thirdPartyToken);
        var user = await identityUserRepository.FindByExternalIdAsync(method, externalUser.ExternalId, false);
        if (user is null)
        {
            return new GenericResponseDto(false, "NOT_FOUND");
        }

        return new GenericResponseDto(true);
    }

    public async Task<PikachuLoginResponseDto> LoginAsync(PikachuLoginInputDto input)
    {
        ValidateLogin(input);

        var selfUrl = configuration["App:SelfUrl"] ?? "";

        var options = new RestClientOptions(selfUrl);
        var restClient = new RestClient(options);
        var request = new RestRequest("/connect/token", Method.Post);

        request.AddHeader("Content-Type", ContentType.FormUrlEncoded);
        request.AddHeader("__tenant", CurrentTenant.Name);

        var param = await SetupLoginParams(input);

        foreach (var kvp in param)
        {
            request.AddParameter(kvp.Key, kvp.Value, ParameterType.GetOrPost);
        }

        var response = await restClient.ExecuteAsync(request);

        using JsonDocument doc = JsonDocument.Parse(response.Content ?? "");
        JsonElement root = doc.RootElement;

        var result = new PikachuLoginResponseDto(response.IsSuccessStatusCode);
        if (result.Success)
        {
            root.TryGetProperty("access_token", out JsonElement accessTokenElement);

            if (accessTokenElement.ValueKind == JsonValueKind.String)
            {
                result.AccessToken = accessTokenElement.GetString();
            }
        }
        root.TryGetProperty("error_description", out JsonElement errorDescriptionElement);
        if (errorDescriptionElement.ValueKind == JsonValueKind.String)
        {
            result.ErrorDescription = errorDescriptionElement.GetString();
        }
        return result;
    }

    private async Task<Dictionary<string, object>> SetupLoginParams(PikachuLoginInputDto input)
    {
        if (input.Method == LoginMethod.UserNameOrPassword)
        {
            return new Dictionary<string, object>
            {
                { "username", input.UserNameOrEmailAddress },
                { "password", input.Password },
                { "grant_type", "password" },
                { "client_id", "Pikachu_App" },
                { "client_secret", "" },
                { "scope", "Pikachu" },
            };
        }

        var externalUser = await SetupExternalUserAsync(input.Method.Value, input.ThirdPartyToken);

        var user = await identityUserRepository.FindByExternalIdAsync(input.Method.Value, externalUser.ExternalId, true);

        return new Dictionary<string, object>
        {
            { "user_id", user!.Id },
            { "grant_type", "user_id" },
            { "client_id", "Third_Party" },
            { "client_secret", "" },
            { "scope", "Pikachu" },
        };
    }

    private static void ValidateLogin(PikachuLoginInputDto input)
    {
        var result = new List<ValidationResult>();
        var requiredMembers = new List<string>();

        if (!input.Method.HasValue)
        {
            requiredMembers.Add(nameof(input.Method));
        }

        if (input.Method == LoginMethod.UserNameOrPassword)
        {
            if (input.UserNameOrEmailAddress.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.UserNameOrEmailAddress));
            }

            if (input.Password.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.Password));
            }
        }
        else
        {
            if (input.ThirdPartyToken.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.ThirdPartyToken));
            }
        }

        if (requiredMembers.Count > 0 || result.Count > 0)
        {
            result.Add(new ValidationResult(errorMessage: "FieldsRequired", memberNames: requiredMembers));
            throw new AbpValidationException(result);
        }
    }

    public async Task<IdentityUserDto> RegisterAsync(PikachuRegisterInputDto input)
    {
        Check.NotNull(input, nameof(input));

        ValidateRegister(input);

        if (input.Method != LoginMethod.UserNameOrPassword)
        {
            var externalUser = await SetupExternalUserAsync(input.Method.Value, input.ThirdPartyToken);
            input.UserName = externalUser.UserName;
            input.Email = externalUser.Email;
            input.Name = externalUser.Name;
            input.ExternalId = externalUser.ExternalId;
        }

        var identityUser = new IdentityUser(GuidGenerator.Create(), input.UserName ?? input.Email, input.Email, CurrentTenant.Id)
        {
            Name = input.Name,
            IsExternal = input.Method != LoginMethod.UserNameOrPassword
        };

        if (!input.PhoneNumber.IsNullOrWhiteSpace())
        {
            identityUser.SetPhoneNumber(input.PhoneNumber, false);
        }

        identityUser.SetBirthday(input.Birthday);
        identityUser.SetMobileNumber(input.MobileNumber);
        identityUser.SetGender(input.Gender);

        if (!input.Role.IsNullOrWhiteSpace())
        {
            var role = (await identityRoleRepository.FindByNormalizedNameAsync(input.Role)) ?? throw new UserFriendlyException("RoleNotFound");
            identityUser.AddRole(role.Id);
        }

        string? property = input.Method switch
        {
            LoginMethod.Facebook => Constant.FacebookId,
            LoginMethod.Google => Constant.GoogleId,
            LoginMethod.Line => Constant.LineId,
            _ => null,
        };

        if (!property.IsNullOrWhiteSpace())
        {
            identityUser.RemoveProperty(property);
            identityUser.SetProperty(property, input.ExternalId);
        }

        var identityResult = await (input.Method == LoginMethod.UserNameOrPassword
            ? identityUserManager.CreateAsync(identityUser, input.Password)
            : identityUserManager.CreateAsync(identityUser));

        identityResult.CheckErrors();

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(identityUser);
    }

    private async Task<ExternalUserDto> SetupExternalUserAsync(LoginMethod loginMethod, string thirdPartyToken)
    {
        var externalUser = new ExternalUserDto();
        if (loginMethod == LoginMethod.Facebook)
        {
            var userInfo = await externalUserAppService.GetFacebookUserDetailsAsync(thirdPartyToken)
                ?? throw new UserFriendlyException(L["UnableToRetrieveUserDetails", "Facebook"].Value);

            externalUser.Name = userInfo.Name;
            externalUser.UserName = userInfo.Id;
            externalUser.Email = userInfo.Email ?? userInfo.Id + "@ibosshops.com";
            externalUser.ExternalId = userInfo.Id;
        }

        if (loginMethod == LoginMethod.Google)
        {
            var userInfo = await externalUserAppService.GetGoogleUserDetailsAsync(thirdPartyToken)
                ?? throw new UserFriendlyException(L["UnableToRetrieveUserDetails", "Google"].Value);

            externalUser.Name = userInfo.GivenName;
            if (!userInfo.FamilyName.IsNullOrWhiteSpace())
            {
                externalUser.Name += " " + userInfo.FamilyName;
            }
            externalUser.UserName = userInfo.Sub;
            externalUser.Email = userInfo.Email ?? userInfo.Sub + "@ibosshops.com";
            externalUser.ExternalId = userInfo.Sub;
        }

        if (loginMethod == LoginMethod.Line)
        {
            var userInfo = await externalUserAppService.GetLineUserDetailsAsync(thirdPartyToken)
                ?? throw new UserFriendlyException(L["UnableToRetrieveUserDetails", "Line"].Value);

            externalUser.Name = userInfo.DisplayName;
            externalUser.UserName = userInfo.UserId;
            externalUser.Email = userInfo.UserId + "@ibosshops.com";
            externalUser.ExternalId = userInfo.UserId;
        }

        return externalUser;
    }

    private static void ValidateRegister(PikachuRegisterInputDto input)
    {
        var result = new List<ValidationResult>();
        var requiredMembers = new List<string>();

        if (!input.Method.HasValue)
        {
            requiredMembers.Add(nameof(input.Method));
        }

        if (input.Method == LoginMethod.UserNameOrPassword)
        {
            if (input.Email.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.Email));
            }

            if (input.UserName.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.UserName));
            }

            if (input.Name.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.Name));
            }

            if (input.Password.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.Password));
            }

            if (input.ConfirmPassword.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.ConfirmPassword));
            }

            if (!string.Equals(input.Password, input.ConfirmPassword, StringComparison.Ordinal))
            {
                result.Add(new ValidationResult(errorMessage: "PasswordsDoNotMatch", memberNames: [nameof(input.Password), nameof(input.ConfirmPassword)]));
            }
        }

        else
        {
            if (input.ThirdPartyToken.IsNullOrWhiteSpace())
            {
                requiredMembers.Add(nameof(input.ThirdPartyToken));
            }
        }

        if (requiredMembers.Count > 0 || result.Count > 0)
        {
            result.Add(new ValidationResult(errorMessage: "FieldsRequired", memberNames: requiredMembers));
            throw new AbpValidationException(result);
        }
    }

    public async Task<GenericResponseDto> SendEmailVerificationCodeAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();

        var user = await identityUserRepository.FindByNormalizedEmailAsync(normalizedEmail);
        if (user != null)
        {
            return new GenericResponseDto(false, L["Pikachu:DuplicateEmail", email].Value);
        }

        var random = new Random();
        var code = random.Next(000000, 999999).ToString();

        memoryCache.Set(VerificationCodePrefix + normalizedEmail, code, CacheEntryOptions);

        string body = File.ReadAllText("wwwroot/EmailTemplates/verification_code.html");
        body ??= $"<p style=\"text-align: center;\">{L["VerificationCodeEmailBody"].Value}</p><h4 style=\"text-align:center\">{code}</h4>";

        var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();
        if (tenantSettings != null)
        {
            body = body
                .Replace("{{logo_url}}", tenantSettings.LogoUrl)
                .Replace("{{facebook_url}}", tenantSettings.Facebook)
                .Replace("{{instagram_url}}", tenantSettings.Instagram)
                .Replace("{{line_url}}", tenantSettings.Line)
                .Replace("{{code}}", code)
                .Replace("{{title}}", "感謝您的註冊");
        }

        try
        {
            await emailSender.SendAsync(email, $"【{tenantSettings?.WebpageTitle}】註冊驗證碼", body);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return new GenericResponseDto(false, ex.Message);
        }
        return new GenericResponseDto(true, null);
    }

    public async Task<VerifyCodeResponseDto> VerifyEmailCodeAsync(string email, string code)
    {
        return await Task.Run(() =>
        {
            var normalizedEmail = email.ToUpperInvariant();

            var verificationCode = memoryCache.Get<string?>(VerificationCodePrefix + normalizedEmail);

            if (verificationCode is null)
            {
                return new VerifyCodeResponseDto(false, email, L["CodeIsExpired"].Value);
            }
            if (!verificationCode.Equals(code))
            {
                return new VerifyCodeResponseDto(false, email, L["CodeDoesnotMatch"].Value);
            }

            memoryCache.Remove(VerificationCodePrefix + email);
            return new VerifyCodeResponseDto(true, email);
        });
    }

    public async Task<GenericResponseDto> SendPasswordResetCodeAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        var user = await identityUserRepository.FindByNormalizedEmailAsync(email);

        if (user is null)
        {
            return new GenericResponseDto(false, L["EmailAddressNotFound"].Value);
        }

        if (!user.IsActive)
        {
            return new GenericResponseDto(false, L["UserIsNotActive"].Value);
        }

        var random = new Random();
        var code = random.Next(000000, 999999).ToString();

        memoryCache.Set(PasswordResetCodePrefix + normalizedEmail, code, CacheEntryOptions);

        memoryCache.Set(VerificationCodePrefix + normalizedEmail, code, CacheEntryOptions);

        string body = File.ReadAllText("wwwroot/EmailTemplates/verification_code.html");
        body ??= $"<p style=\"text-align: center;\">{L["ResetPasswordCodeEmailBody"].Value}</p><h4 style=\"text-align:center\">{code}</h4>";

        var tenantSettings = await tenantSettingsAppService.FirstOrDefaultAsync();
        if (tenantSettings != null)
        {
            body = body
                .Replace("{{logo_url}}", tenantSettings.LogoUrl)
                .Replace("{{facebook_url}}", tenantSettings.Facebook)
                .Replace("{{instagram_url}}", tenantSettings.Instagram)
                .Replace("{{line_url}}", tenantSettings.Line)
                .Replace("{{code}}", code)
                .Replace("{{title}}", "重設密碼");
        }

        await emailSender.SendAsync(email, $"【{tenantSettings?.WebpageTitle}】重設密碼驗證碼", body);
        return new GenericResponseDto(true);
    }

    public async Task<VerifyCodeResponseDto> VerifyPasswordResetCodeAsync(string email, string code)
    {
        var normalizedEmail = email.ToUpperInvariant();

        var passwordResetCode = memoryCache.Get<string?>(PasswordResetCodePrefix + normalizedEmail);

        if (passwordResetCode is null)
        {
            return new VerifyCodeResponseDto(false, email, L["CodeIsExpired"].Value);
        }
        if (!passwordResetCode.Equals(code))
        {
            return new VerifyCodeResponseDto(false, email, L["CodeDoesnotMatch"].Value);
        }

        var user = await identityUserRepository.FindByNormalizedEmailAsync(normalizedEmail);
        if (user is null)
        {
            return new VerifyCodeResponseDto(false, email, L["EmailAddressNotFound"].Value);
        }

        memoryCache.Remove(PasswordResetCodePrefix + email);

        var resetToken = await identityUserManager.GeneratePasswordResetTokenAsync(user);

        return new VerifyCodeResponseDto(true, email)
        {
            ResetToken = resetToken,
        };
    }

    public async Task<GenericResponseDto> ChangePasswordAsync(Guid id, ChangePasswordInput input)
    {
        var user = await identityUserRepository.GetAsync(id);
        (await identityUserManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword)).CheckErrors();
        return new GenericResponseDto(true);
    }

    public async Task<GenericResponseDto> ResetPasswordAsync(PikachuResetPasswordDto input)
    {
        var user = await identityUserRepository.FindByNormalizedEmailAsync(input.Email);

        if (user is null)
        {
            return new GenericResponseDto(false, L["EmailAddressNotFound"].Value);
        }

        (await identityUserManager.ResetPasswordAsync(user, input.ResetToken, input.Password)).CheckErrors();
        return new GenericResponseDto(true);
    }
}
