using Asp.Versioning;
using Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.WebsiteBasicSettings;

[RemoteService(IsEnabled = true)]
[ControllerName("WebsiteBasicSettings")]
[Area("app")]
[Route("api/app/website-basic-settings")]
public class WebsiteBasicSettingController(IWebsiteBasicSettingAppService websiteBasicSettingAppService) : PikachuController, IWebsiteBasicSettingAppService
{
    [HttpGet]
    public Task<WebsiteBasicSettingDto> FirstOrDefaultAsync()
    {
        return websiteBasicSettingAppService.FirstOrDefaultAsync();
    }

    [HttpPost("set-enabled/{isEnabled}")]
    public Task<WebsiteBasicSettingDto> SetIsEnabledAsync(bool isEnabled)
    {
        return websiteBasicSettingAppService.SetIsEnabledAsync(isEnabled);
    }

    [HttpPost]
    public Task<WebsiteBasicSettingDto> UpdateAsync(UpdateWebsiteBasicSettingDto input)
    {
        return websiteBasicSettingAppService.UpdateAsync(input);
    }
}
