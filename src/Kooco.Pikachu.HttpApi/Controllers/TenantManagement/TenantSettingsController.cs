using Asp.Versioning;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.TenantManagement;

[RemoteService(IsEnabled = true)]
[ControllerName("TenantSettings")]
[Area("app")]
[Route("api/app/tenant-settings")]
public class TenantSettingsController(ITenantSettingsAppService tenantSettingsAppService) : PikachuController, ITenantSettingsAppService
{
    [HttpGet("default")]
    public Task<TenantSettingsDto> FirstOrDefaultAsync()
    {
        return tenantSettingsAppService.FirstOrDefaultAsync();
    }

    [HttpPost("upload-image")]
    public Task<string> UploadImageAsync(UploadImageDto input)
    {
        return tenantSettingsAppService.UploadImageAsync(input);
    }

    [HttpDelete("{blobName}")]
    public Task DeleteImageAsync(string blobName)
    {
        return tenantSettingsAppService.DeleteImageAsync(blobName);
    }

    [HttpPost("tenant-information")]
    public Task<TenantInformationDto> UpdateTenantInformationAsync(UpdateTenantInformationDto input)
    {
        return tenantSettingsAppService.UpdateTenantInformationAsync(input);
    }

    [HttpGet("tenant-information")]
    public Task<TenantInformationDto> GetTenantInformationAsync()
    {
        return tenantSettingsAppService.GetTenantInformationAsync();
    }

    [HttpPost("customer-service")]
    public Task<TenantCustomerServiceDto> UpdateTenantCustomerServiceAsync(UpdateTenantCustomerServiceDto input)
    {
        return tenantSettingsAppService.UpdateTenantCustomerServiceAsync(input);
    }

    [HttpGet("customer-service")]
    public Task<TenantCustomerServiceDto> GetTenantCustomerServiceAsync()
    {
        return tenantSettingsAppService.GetTenantCustomerServiceAsync();
    }

    [HttpPost("terms-and-conditions")]
    public Task<string?> UpdateTenantTermsAndConditionsAsync([Required] string termsAndConditions)
    {
        return tenantSettingsAppService.UpdateTenantTermsAndConditionsAsync(termsAndConditions);
    }

    [HttpGet("terms-and-conditions")]
    public Task<string?> GetTenantTermsAndConditionsAsync()
    {
        return tenantSettingsAppService.GetTenantTermsAndConditionsAsync();
    }

    [HttpPost("privacy-policy")]
    public Task<string?> UpdateTenantPrivacyPolicyAsync([Required] string privacyPolicy)
    {
        return tenantSettingsAppService.UpdateTenantPrivacyPolicyAsync(privacyPolicy);
    }

    [HttpGet("privacy-policy")]
    public Task<string?> GetTenantPrivacyPolicyAsync()
    {
        return tenantSettingsAppService.GetTenantPrivacyPolicyAsync();
    }

    [HttpPost("frontend-information")]
    public Task<TenantFrontendInformationDto> UpdateTenantFrontendInformationAsync(UpdateTenantFrontendInformationDto input)
    {
        return tenantSettingsAppService.UpdateTenantFrontendInformationAsync(input);
    }

    [HttpGet("frontend-information")]
    public Task<TenantFrontendInformationDto> GetTenantFrontendInformationAsync()
    {
        return tenantSettingsAppService.GetTenantFrontendInformationAsync();
    }

    [HttpPost("social-media")]
    public Task<TenantSocialMediaDto> UpdateTenantSocialMediaAsync(UpdateTenantSocialMediaDto input)
    {
        return tenantSettingsAppService.UpdateTenantSocialMediaAsync(input);
    }

    [HttpGet("social-media")]
    public Task<TenantSocialMediaDto> GetTenantSocialMediaAsync()
    {
        return tenantSettingsAppService.GetTenantSocialMediaAsync();
    }

    [HttpPost("google-tag-manager")]
    public Task<TenantGoogleTagManagerDto> UpdateTenantGoogleTagManagerAsync(UpdateTenantGoogleTagManagerDto input)
    {
        return tenantSettingsAppService.UpdateTenantGoogleTagManagerAsync(input);
    }

    [HttpGet("google-tag-manager")]
    public Task<TenantGoogleTagManagerDto> GetTenantGoogleTagManagerAsync()
    {
        return tenantSettingsAppService.GetTenantGoogleTagManagerAsync();
    }
}
