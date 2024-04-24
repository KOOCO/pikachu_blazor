using Asp.Versioning;
using Kooco.Pikachu.TenantEmailing;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.TenantEmailing;

[RemoteService(IsEnabled = true)]
[ControllerName("TenantEmailSettings")]
[Area("app")]
[Route("api/app/tenant-email-settings")]
public class TenantEmailSettingsController(
    ITenantEmailSettingsAppService _tenantEmailSettingsAppService
    ) : AbpController, ITenantEmailSettingsAppService
{
    [HttpGet]
    public Task<TenantEmailSettingsDto> GetEmailSettingsAsync()
    {
        return _tenantEmailSettingsAppService.GetEmailSettingsAsync();
    }

    [HttpPut]
    public Task UpdateEmailSettingsAsync(CreateUpdateTenantEmailSettingsDto input)
    {
        return _tenantEmailSettingsAppService.UpdateEmailSettingsAsync(input);
    }
}
