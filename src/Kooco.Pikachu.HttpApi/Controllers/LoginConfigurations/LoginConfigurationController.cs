using Asp.Versioning;
using Kooco.Pikachu.LoginConfigurations;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.LoginConfigurations;

[ControllerName("LoginConfigurations")]
[RemoteService(IsEnabled = true)]
[Area("app")]
[Route("api/app/login-configurations")]
public class LoginConfigurationController(ILoginConfigurationAppService loginConfigurationAppService) : PikachuController, ILoginConfigurationAppService
{
    [HttpDelete]
    public Task DeleteAsync()
    {
        return loginConfigurationAppService.DeleteAsync();
    }

    [HttpGet]
    public Task<LoginConfigurationDto?> FirstOrDefaultAsync()
    {
        return loginConfigurationAppService.FirstOrDefaultAsync();
    }

    [HttpPost]
    public Task UpdateAsync(UpdateLoginConfigurationDto input)
    {
        return loginConfigurationAppService.UpdateAsync(input);
    }
}
