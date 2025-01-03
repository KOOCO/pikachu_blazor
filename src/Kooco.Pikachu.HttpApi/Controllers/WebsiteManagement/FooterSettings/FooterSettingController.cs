using Asp.Versioning;
using Kooco.Pikachu.WebsiteManagement.FooterSettings;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.WebsiteManagement.FooterSettings;

[RemoteService(IsEnabled = true)]
[ControllerName("FooterSettings")]
[Area("app")]
[Route("api/app/footer-settings")]
public class FooterSettingController(IFooterSettingAppService footerSettingAppService) : PikachuController, IFooterSettingAppService
{
    [HttpGet]
    public Task<FooterSettingDto?> FirstOrDefaultAsync()
    {
        return footerSettingAppService.FirstOrDefaultAsync();
    }

    [HttpPost]
    public Task<FooterSettingDto> UpdateAsync(UpdateFooterSettingDto input)
    {
        return footerSettingAppService.UpdateAsync(input);
    }
}
