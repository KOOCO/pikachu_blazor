using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PikachuAccounts.ExternalUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    IIdentityUserRepository identityUserRepository, IExternalUserAppService externalUserAppService) : PikachuAppService, IPikachuAccountAppService
{
    public const string VerificationCodePrefix = "__VerificationCode:";
    public const string PasswordResetCodePrefix = "__ResetPasswordCode:";
    public MemoryCacheEntryOptions CacheEntryOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
    };

    public async Task<PikachuLoginResponseDto> LoginAsync(PikachuLoginInputDto input)
    {
        var selfUrl = configuration["App:SelfUrl"] ?? "";

        var options = new RestClientOptions(selfUrl);
        var restClient = new RestClient(options);
        var request = new RestRequest("/connect/token", Method.Post);

        request.AddHeader("Content-Type", ContentType.FormUrlEncoded);

        var param = new Dictionary<string, object>
        {
            { "username", input.UserNameOrEmailAddress },
            { "password", input.Password },
            { "grant_type", "password" },
            { "client_id", "Pikachu_App" },
            { "client_secret", "" },
            { "scope", "Pikachu" },
        };

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

    public async Task<IdentityUserDto> RegisterAsync(PikachuRegisterInputDto input)
    {
        Check.NotNull(input, nameof(input));

        ValidateRegister(input);

        if (input.Method == LoginMethod.UserNameOrPassword)
        {
            return await RegisterLocalUserAsync(input);
        }
        else
        {
            return await RegisterExternalUserAsync(input);
        }
    }

    private async Task<IdentityUserDto> RegisterLocalUserAsync(PikachuRegisterInputDto input)
    {
        var identityUser = new IdentityUser(GuidGenerator.Create(), input.UserName, input.Email, CurrentTenant.Id)
        {
            Name = input.Name
        };

        if (!input.PhoneNumber.IsNullOrWhiteSpace())
        {
            identityUser.SetPhoneNumber(input.PhoneNumber, false);
        }
        if (input.Birthday.HasValue)
        {
            identityUser.SetProperty(Constant.Birthday, input.Birthday);
        }

        if (!input.Role.IsNullOrWhiteSpace())
        {
            var role = (await identityRoleRepository.FindByNormalizedNameAsync(input.Role)) ?? throw new UserFriendlyException("RoleNotFound");
            identityUser.AddRole(role.Id);
        }

        (await identityUserManager.CreateAsync(identityUser, input.Password)).CheckErrors();

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(identityUser);
    }

    private async Task<IdentityUserDto> RegisterExternalUserAsync(PikachuRegisterInputDto input)
    {
        string email = "";
        if (input.Method == LoginMethod.Facebook)
        {
            var userInfo = await externalUserAppService.GetFacebookUserDetailsAsync(input.ThirdPartyToken)
                ?? throw new UserFriendlyException(L["UnableToRetrieveUserDetails", "Facebook"].Value);

            email = userInfo.Email;
        }

        if (input.Method == LoginMethod.Google)
        {
            var userInfo = await externalUserAppService.GetGoogleUserDetailsAsync(input.ThirdPartyToken)
                ?? throw new UserFriendlyException(L["UnableToRetrieveUserDetails", "Google"].Value);

            email = userInfo.Email;
        }

        if (input.Method == LoginMethod.Line)
        {
            var userInfo = await externalUserAppService.GetLineUserDetailsAsync(input.ThirdPartyToken)
                ?? throw new UserFriendlyException(L["UnableToRetrieveUserDetails", "Line"].Value);

            email = userInfo.DisplayName;
        }

        var identityUser = new IdentityUser(GuidGenerator.Create(), email, email, CurrentTenant.Id)
        {
            Name = input.Name
        };

        if (!input.Role.IsNullOrWhiteSpace())
        {
            var role = (await identityRoleRepository.FindByNormalizedNameAsync(input.Role)) ?? throw new UserFriendlyException("RoleNotFound");
            identityUser.AddRole(role.Id);
        }

        (await identityUserManager.CreateAsync(identityUser)).CheckErrors();

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(identityUser);
    }

    private static void ValidateRegister(PikachuRegisterInputDto input)
    {
        var result = new List<ValidationResult>();
        var requiredMembers = new List<string>();

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

    public async Task SendEmailVerificationCodeAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        var random = new Random();
        var code = random.Next(000000, 999999).ToString();

        memoryCache.Set(VerificationCodePrefix + normalizedEmail, code, CacheEntryOptions);

        string body = $"<p style=\"text-align: center;\">{L["VerificationCodeEmailBody"].Value}</p><h4 style=\"text-align:center\">{code}</h4>";

        await emailSender.SendAsync(email, L["VerificationToken"].Value, body);
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

        string body = $"<p style=\"text-align: center;\">{L["ResetPasswordCodeEmailBody"].Value}</p><h4 style=\"text-align:center\">{code}</h4>";

        await emailSender.SendAsync(email, L["ResetPasswordCode"].Value, body);
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
