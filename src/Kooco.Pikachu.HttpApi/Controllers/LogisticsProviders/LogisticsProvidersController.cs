using Kooco.Pikachu.LogisticsProviders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.LogisticsProviders;

[RemoteService(IsEnabled = true)]
[ControllerName("LogisticsProviders")]
[Area("app")]
[Route("api/app/logistics-providers")]
public class LogisticsProvidersController(
    ILogisticsProvidersAppService _logisticsProvidersAppService
    ) : AbpController, ILogisticsProvidersAppService
{
    [HttpGet]
    public Task<List<LogisticsProviderSettingsDto>> GetAllAsync()
    {
        return _logisticsProvidersAppService.GetAllAsync();
    }

    [HttpPut("green-world")]
    public Task UpdateGreenWorldAsync(GreenWorldLogisticsCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateGreenWorldAsync(input);
    }

    [HttpPut("home-delivery")]
    public Task UpdateHomeDeliveryAsync(HomeDeliveryCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateHomeDeliveryAsync(input);
    }
}
