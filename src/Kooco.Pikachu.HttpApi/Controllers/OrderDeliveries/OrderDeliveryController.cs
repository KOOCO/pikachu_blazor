using Kooco.Pikachu.OrderDeliveries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.OrderDeliveries;

[RemoteService(IsEnabled = true)]
[ControllerName("OrderDeliveries")]
[Area("app")]
[Route("api/app/order-deliveries")]
public class OrderDeliveryController(
    IOrderDeliveryAppService _orderDeliveryAppService
    ) : AbpController, IOrderDeliveryAppService
{
    [HttpGet]
    public Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid Id)
    {
        return _orderDeliveryAppService.GetListByOrderAsync(Id);
    }
}
