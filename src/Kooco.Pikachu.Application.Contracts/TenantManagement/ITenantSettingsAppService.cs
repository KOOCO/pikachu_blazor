using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantManagement;

public interface ITenantSettingsAppService : IApplicationService
{
    Task<TenantSettingsDto> UpdateAsync(UpdateTenantSettingsDto input);
    Task<TenantSettingsDto?> FirstOrDefaultAsync();
    Task<string> UploadImageAsync(UploadImageDto input);
    Task DeleteImageAsync(string blobName);
}
