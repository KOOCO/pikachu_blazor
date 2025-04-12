using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Tenants;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.TenantSettings.Default)]
public class TenantSettingsAppService(TenantSettingsManager tenantSettingsManager, ImageContainerManager imageContainerManager) : PikachuAppService, ITenantSettingsAppService
{
    [AllowAnonymous]
    public async Task<TenantSettingsDto> FirstOrDefaultAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return ObjectMapper.Map<TenantSettings, TenantSettingsDto>(tenantSettings);
    }

    public async Task<string> UploadImageAsync(UploadImageDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotNullOrWhiteSpace(input.Base64, nameof(input.Base64));
        Check.NotNullOrWhiteSpace(input.FileName, nameof(input.FileName));

        string blobName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(input.FileName);
        byte[] fileBytes = Convert.FromBase64String(input.Base64);

        var url = await imageContainerManager.SaveAsync(blobName, fileBytes, true);
        return url;
    }

    public async Task DeleteImageAsync(string blobName)
    {
        await imageContainerManager.DeleteAsync(blobName);
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<TenantInformationDto> UpdateTenantInformationAsync(UpdateTenantInformationDto input)
    {
        Check.NotNull(input, nameof(input));

        var tenantSettings = await tenantSettingsManager.UpdateTenantInformationAsync(input.ShortCode, input.TenantUrl, input.Domain,
            input.TenantContactTitle, input.TenantContactPerson, input.ContactPhone, input.TenantContactEmail);

        return ObjectMapper.Map<TenantSettings, TenantInformationDto>(tenantSettings);
    }

    public async Task<TenantInformationDto> GetTenantInformationAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return ObjectMapper.Map<TenantSettings, TenantInformationDto>(tenantSettings);
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<TenantCustomerServiceDto> UpdateTenantCustomerServiceAsync(UpdateTenantCustomerServiceDto input)
    {
        Check.NotNull(input, nameof(input));

        var tenantSettings = await tenantSettingsManager.UpdateCustomerServiceAsync(input.CompanyName, input.BusinessRegistrationNumber,
            input.CustomerServiceEmail, input.CustomerServiceContactPhone, input.ServiceHoursFrom, input.ServiceHoursTo);

        return ObjectMapper.Map<TenantSettings, TenantCustomerServiceDto>(tenantSettings);
    }

    [AllowAnonymous]
    public async Task<TenantCustomerServiceDto> GetTenantCustomerServiceAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return ObjectMapper.Map<TenantSettings, TenantCustomerServiceDto>(tenantSettings);
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<string?> UpdateTenantTermsAndConditionsAsync([Required] string termsAndConditions)
    {
        var tenantSettings = await tenantSettingsManager.UpdateTermsAndConditionsAsync(termsAndConditions);
        return tenantSettings.TermsAndConditions;
    }

    [AllowAnonymous]
    public async Task<string?> GetTenantTermsAndConditionsAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return tenantSettings.TermsAndConditions;
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<string?> UpdateTenantPrivacyPolicyAsync([Required] string privacyPolicy)
    {
        var tenantSettings = await tenantSettingsManager.UpdatePrivacyPolicyAsync(privacyPolicy);
        return tenantSettings.PrivacyPolicy;
    }

    [AllowAnonymous]
    public async Task<string?> GetTenantPrivacyPolicyAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return tenantSettings.PrivacyPolicy;
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<TenantFrontendInformationDto> UpdateTenantFrontendInformationAsync(UpdateTenantFrontendInformationDto input)
    {
        var tenantSettings = await tenantSettingsManager.UpdateTenantFrontendInformationAsync(input.WebpageTitle, input.FaviconUrl, input.LogoUrl, input.BannerUrl, input.Description);

        return ObjectMapper.Map<TenantSettings, TenantFrontendInformationDto>(tenantSettings);
    }

    [AllowAnonymous]
    public async Task<TenantFrontendInformationDto> GetTenantFrontendInformationAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return ObjectMapper.Map<TenantSettings, TenantFrontendInformationDto>(tenantSettings);
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<TenantSocialMediaDto> UpdateTenantSocialMediaAsync(UpdateTenantSocialMediaDto input)
    {
        var tenantSettings = await tenantSettingsManager.UpdateTenantSocialMediaAsync(input.FacebookDisplayName, input.FacebookLink, input.InstagramDisplayName,input.InstagramLink, input.LineDisplayName,input.LineLink);

        return ObjectMapper.Map<TenantSettings, TenantSocialMediaDto>(tenantSettings);
    }

    [AllowAnonymous]
    public async Task<TenantSocialMediaDto> GetTenantSocialMediaAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return ObjectMapper.Map<TenantSettings, TenantSocialMediaDto>(tenantSettings);
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<TenantGoogleTagManagerDto> UpdateTenantGoogleTagManagerAsync(UpdateTenantGoogleTagManagerDto input)
    {
        var tenantSettings = await tenantSettingsManager.UpdateTenantGoogleTagManagerAsync(input.GtmEnabled, input.GtmContainerId);

        return ObjectMapper.Map<TenantSettings, TenantGoogleTagManagerDto>(tenantSettings);
    }

    [AllowAnonymous]
    public async Task<TenantGoogleTagManagerDto> GetTenantGoogleTagManagerAsync()
    {
        var tenantSettings = await tenantSettingsManager.GetAsync();

        return ObjectMapper.Map<TenantSettings, TenantGoogleTagManagerDto>(tenantSettings);
    }
}
