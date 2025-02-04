using Asp.Versioning;
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

    [HttpPost("generate-deliveryNumber-SelfPickup-HomeDelivery")]
    public Task GenerateDeliveryNumberForSelfPickupAndHomeDeliveryAsync(Guid orderId, Guid orderDeliveryId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("find-status")]
    public Task<string> FindStatusAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("print-shipping-label")]
    public Task<string> OnPrintShippingLabel(OrderDto order, OrderDeliveryDto orderDelivery)
    {
        throw new NotImplementedException();
    }

    [HttpGet("batch-printing-shipping-label")]
    public Task<string> OnBatchPrintingShippingLabel(List<string> allPayLogisticsId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("batch-printing-shipping")]
    public Task<Tuple<List<string>, List<string>, List<string>>> OnBatchPrintingShippingLabel(Dictionary<string, string> allPayLogisticsIds, Dictionary<string, string>? DeliveryNumbers,
        Dictionary<string, string>? allPayLogisticsForTCat711)
    {
        throw new NotImplementedException();
    }
    [HttpPost("IssueInvoice")]
    public Task IssueInvoiceAync(Guid orderId)
    {
        throw new NotImplementedException();
    }
}
