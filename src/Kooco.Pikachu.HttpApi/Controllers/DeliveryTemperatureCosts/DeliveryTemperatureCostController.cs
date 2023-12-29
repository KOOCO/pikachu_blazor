using Kooco.Pikachu.DeliveryTemperatureCosts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.DeliveryTemperatureCosts;

[RemoteService(IsEnabled = true)]
[ControllerName("DeliveryTemperatureCosts")]
[Area("app")]
[Route("api/app/delivery-temperature-costs")]
public class DeliveryTemperatureCostController (
    IDeliveryTemperatureCostAppService _deliveryTemperatureCostAppService
    ) : AbpController, IDeliveryTemperatureCostAppService
{
    [HttpGet]
    public Task<List<DeliveryTemperatureCostDto>> GetListAsync()
    {
        return _deliveryTemperatureCostAppService.GetListAsync();
    }

    [HttpPut]
    public Task UpdateCostAsync(List<UpdateDeliveryTemperatureCostDto> Costs)
    {
        return _deliveryTemperatureCostAppService.UpdateCostAsync(Costs);
    }
}
