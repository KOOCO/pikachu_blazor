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
    [HttpPut("post-office")]
    public Task UpdatePostOfficeAsync(PostOfficeCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdatePostOfficeAsync(input);
    }
    [HttpPut("7-11")]
    public Task UpdateSevenToElevenAsync(SevenToElevenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateSevenToElevenAsync(input);
    }
    [HttpPut("family-mart")]
    public Task UpdateFamilyMartAsync(SevenToElevenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateFamilyMartAsync(input);
    }
    [HttpPut("7-11frozen")]
    public Task UpdateSevenToElevenFrozenAsync(SevenToElevenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateSevenToElevenFrozenAsync(input);
    }
    [HttpPut("b-normal")]
    public Task UpdateBNormalAsync(BNormalCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateBNormalAsync(input);
    }
    [HttpPut("b-frozen")]
    public Task UpdateBFrozenAsync(BNormalCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateBFrozenAsync(input);
    }
    [HttpPut("b-freeze")]
    public Task UpdateBFreezeAsync(BNormalCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateBFreezeAsync(input);
    }
}
