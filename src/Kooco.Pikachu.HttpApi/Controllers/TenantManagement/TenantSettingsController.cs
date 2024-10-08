using Asp.Versioning;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.TenantManagement;

[RemoteService(IsEnabled = true)]
[ControllerName("TenantSettings")]
[Area("app")]
[Route("api/app/tenant-settings")]
public class TenantSettingsController(ITenantSettingsAppService tenantset) : PikachuController, ITenantSettingsAppService
{
    [HttpDelete("{blobName}")]
    public Task DeleteImageAsync(string blobName)
    {
        return tenantset.DeleteImageAsync(blobName);
    }

    [HttpGet("default")]
    public Task<TenantSettingsDto?> FirstOrDefaultAsync()
    {
        return tenantset.FirstOrDefaultAsync();
    }

    [HttpPost]
    public Task<TenantSettingsDto> UpdateAsync(UpdateTenantSettingsDto input)
    {
        return tenantset.UpdateAsync(input);
    }

    [HttpPost("upload-image")]
    public Task<string> UploadImageAsync(UploadImageDto input)
    {
        return tenantset.UploadImageAsync(input);
    }
}
