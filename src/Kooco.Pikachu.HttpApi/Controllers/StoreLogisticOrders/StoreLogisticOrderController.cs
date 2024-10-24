﻿using Asp.Versioning;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.StoreLogisticOrders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.StoreLogisticOrders;

[RemoteService(IsEnabled = true)]
[ControllerName("StoreLogisticOrders")]
[Area("app")]
[Route("api/app/store-logistic-order")]
public class StoreLogisticOrderController(
    IStoreLogisticsOrderAppService _storeLogisticsOrderAppService
) : AbpController, IStoreLogisticsOrderAppService
{
    [HttpPost("create-homedelivery-shipment")]
    public Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
        return _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDeliveryId, deliveryMethod);
    }
    [HttpPost("create-logistic")]
    public Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
        return _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(orderId,  orderDeliveryId, deliveryMethod);
    }

    [HttpGet("generate-deliveryNumber-for-tCatDelivery")]
    public Task<PrintObtResponse?> GenerateDeliveryNumberForTCatDeliveryAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
        throw new NotImplementedException();
    }

    [HttpGet("generate-deliveryNumber-fortCatDelivery711")]
    public Task<PrintOBTB2SResponse?> GenerateDeliveryNumberForTCat711DeliveryAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null)
    {
        throw new NotImplementedException();
    }

    [HttpGet("get-store/{deliveryMethod}")]
    public Task<EmapApiResponse> GetStoreAsync(string deliveryMethod)
    {
        return _storeLogisticsOrderAppService.GetStoreAsync(deliveryMethod);
    }
}
