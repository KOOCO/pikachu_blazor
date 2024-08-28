using Asp.Versioning;
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
    [HttpPut("7-11C2C")]
    public Task UpdateSevenToElevenC2CAsync(SevenToElevenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateSevenToElevenC2CAsync(input);
    }
    [HttpPut("family-mart")]
    public Task UpdateFamilyMartAsync(SevenToElevenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateFamilyMartAsync(input);
    }
    [HttpPut("family-mart-c2c")]
    public Task UpdateFamilyMartC2CAsync(SevenToElevenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateFamilyMartC2CAsync(input);
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
    [HttpPut("green-world-c2c")]
    public Task UpdateGreenWorldC2CAsync(GreenWorldLogisticsCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateGreenWorldC2CAsync(input);
    }

    [HttpPut("t-cat")]
    public Task UpdateTCatAsync(TCatLogisticsCreateUpdateDto entity)
    {
        return _logisticsProvidersAppService.UpdateTCatAsync(entity);
    }

    [HttpPut("t-cat-normal")]
    public Task UpdateTCatNormalAsync(TCatNormalCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateTCatNormalAsync(input);
    }

    [HttpPut("t-cat-freeze")]
    public Task UpdateTCatFreezeAsync(TCatFreezeCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateTCatFreezeAsync(input);
    }

    [HttpPut("t-cat-frozen")]
    public Task UpdateTCatFrozenAsync(TCatFrozenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateTCatFrozenAsync(input);
    }

    [HttpPut("t-cat-711-normal")]
    public Task UpdateTCat711NormalAsync(TCat711NormalCreateUpdate input)
    {
        return _logisticsProvidersAppService.UpdateTCat711NormalAsync(input);
    }

    [HttpPut("t-cat-711-freeze")]
    public Task UpdateTCat711FreezeAsync(TCat711FreezeCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateTCat711FreezeAsync(input);
    }

    [HttpPut("t-cat-711-frozen")]
    public Task UpdateTCat711FrozenAsync(TCat711FrozenCreateUpdateDto input)
    {
        return _logisticsProvidersAppService.UpdateTCat711FrozenAsync(input);
    }
}
