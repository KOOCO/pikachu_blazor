using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Tenants;

public interface ITenantSettingsAppService : IApplicationService
{
    Task<TenantSettingsDto> FirstOrDefaultAsync();
    Task<string> UploadImageAsync(UploadImageDto input);
    Task DeleteImageAsync(string blobName);

    Task<TenantInformationDto> UpdateTenantInformationAsync(UpdateTenantInformationDto input);
    Task<TenantInformationDto> GetTenantInformationAsync();
    Task<TenantCustomerServiceDto> UpdateTenantCustomerServiceAsync(UpdateTenantCustomerServiceDto input);
    Task<TenantCustomerServiceDto> GetTenantCustomerServiceAsync();
    Task<string?> UpdateTenantTermsAndConditionsAsync([Required] string termsAndConditions);
    Task<string?> GetTenantTermsAndConditionsAsync();
    Task<string?> UpdateTenantPrivacyPolicyAsync([Required] string privacyPolicy);
    Task<string?> GetTenantPrivacyPolicyAsync();
    Task<TenantFrontendInformationDto> UpdateTenantFrontendInformationAsync(UpdateTenantFrontendInformationDto input);
    Task<TenantFrontendInformationDto> GetTenantFrontendInformationAsync();
    Task<TenantSocialMediaDto> UpdateTenantSocialMediaAsync(UpdateTenantSocialMediaDto input);
    Task<TenantSocialMediaDto> GetTenantSocialMediaAsync();
    Task<TenantGoogleTagManagerDto> UpdateTenantGoogleTagManagerAsync(UpdateTenantGoogleTagManagerDto input);
    Task<TenantGoogleTagManagerDto> GetTenantGoogleTagManagerAsync();
}
