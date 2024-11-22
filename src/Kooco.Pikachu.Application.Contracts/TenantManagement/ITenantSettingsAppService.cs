using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantManagement;

public interface ITenantSettingsAppService : IApplicationService
{
    Task<TenantSettingsDto> UpdateAsync(UpdateTenantSettingsDto input);
    Task<TenantSettingsDto?> FirstOrDefaultAsync();
    Task<string> UploadImageAsync(UploadImageDto input);
    Task DeleteImageAsync(string blobName);

    Task<TenantInformationDto> UpdateTenantInformationAsync(UpdateTenantInformationDto input);
    Task<TenantInformationDto> GetTenantInformationAsync();
    Task<CustomerServiceDto> UpdateCustomerServiceAsync(UpdateCustomerServiceDto input);
    Task<CustomerServiceDto> GetCustomerServiceAsync();
    Task<string?> UpdateTenantPrivacyPolicyAsync([Required] string privacyPolicy);
    Task<string?> GetTenantPrivacyPolicyAsync();
    Task<TenantFrontendInformationDto> UpdateTenantFrontendInformationAsync(UpdateTenantFrontendInformationDto input);
    Task<TenantFrontendInformationDto> GetTenantFrontendInformationAsync();
}
