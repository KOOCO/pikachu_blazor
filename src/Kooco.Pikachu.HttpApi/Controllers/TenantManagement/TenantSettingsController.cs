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
    [HttpDelete("{blobName}")]
    public Task DeleteImageAsync(string blobName)
    {
        return tenantSettingsAppService.DeleteImageAsync(blobName);
    }

    [HttpGet("default")]
    public Task<TenantSettingsDto?> FirstOrDefaultAsync()
    {
        return tenantSettingsAppService.FirstOrDefaultAsync();
    }

    [HttpGet("customer-service")]
    public Task<TenantCustomerServiceDto> GetTenantCustomerServiceAsync()
    {
        return tenantSettingsAppService.GetTenantCustomerServiceAsync();
    }

    [HttpGet("privacy-policy")]
    public Task<string?> GetTenantPrivacyPolicyAsync()
    {
        return tenantSettingsAppService.GetTenantPrivacyPolicyAsync();
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

    [HttpPost("privacy-policy")]
    public Task<string?> UpdateTenantPrivacyPolicyAsync([Required] string privacyPolicy)
    {
        return tenantSettingsAppService.UpdateTenantPrivacyPolicyAsync(privacyPolicy);
    }

    [HttpPost("tenant-information")]
    public Task<TenantInformationDto> UpdateTenantInformationAsync(UpdateTenantInformationDto input)
    {
        return tenantSettingsAppService.UpdateTenantInformationAsync(input);
    }

    [HttpPost("upload-image")]
    public Task<string> UploadImageAsync(UploadImageDto input)
    {
        return tenantSettingsAppService.UploadImageAsync(input);
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
