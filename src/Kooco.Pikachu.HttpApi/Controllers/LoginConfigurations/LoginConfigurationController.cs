using Asp.Versioning;
using Kooco.Pikachu.LoginConfigurations;
using Microsoft.AspNetCore.Mvc;
using System;
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

    [HttpGet("{tenantId}")]
    public Task<LoginConfigurationDto?> FirstOrDefaultAsync(Guid? tenantId)
    {
        return loginConfigurationAppService.FirstOrDefaultAsync(tenantId);
    }

    [HttpPost]
    public Task UpdateAsync(UpdateLoginConfigurationDto input)
    {
        return loginConfigurationAppService.UpdateAsync(input);
    }
}
