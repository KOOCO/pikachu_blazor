using Asp.Versioning;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
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
    public  Task<OrderDeliveryDto> GetDeliveryOrderAsync(Guid Id)
    {
     return _orderDeliveryAppService.GetDeliveryOrderAsync(Id);
    }

    [HttpGet]
    public Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid Id)
    {
        return _orderDeliveryAppService.GetListByOrderAsync(Id);
    }
    [HttpPut("update-delivery-status/{id}")]
    public Task UpdateOrderDeliveryStatus(Guid Id)
    {
        return _orderDeliveryAppService.UpdateOrderDeliveryStatus(Id);
    }

    [HttpPut("update-shipping/{id}")]
    public Task<OrderDeliveryDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
    {
        return _orderDeliveryAppService.UpdateShippingDetails(id, input);
    }
}
