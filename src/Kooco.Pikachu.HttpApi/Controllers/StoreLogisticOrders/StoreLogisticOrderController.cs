using Asp.Versioning;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.StoreLogisticOrders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.StoreLogisticOrders
{
    [RemoteService(IsEnabled = true)]
    [ControllerName("StoreLogisticOrders")]
    [Area("app")]
    [Route("api/app/store-logistic-order")]
    public class StoreLogisticOrderController(
    IStoreLogisticsOrderAppService _storeLogisticsOrderAppService
    ) : AbpController, IStoreLogisticsOrderAppService
    {
        [HttpPost("create-homedelivery-shipment")]
        public Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId)
        {
            return _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDeliveryId);
        }
        [HttpPost("create-logistic")]
        public Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(Guid orderId, Guid orderDeliveryId)
        {

            return _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync( orderId,  orderDeliveryId);
        }
        [HttpGet("get-store/{deliveryMethod}")]
        public Task<EmapApiResponse> GetStoreAsync(string deliveryMethod)
        {
            return _storeLogisticsOrderAppService.GetStoreAsync(deliveryMethod);
        }
    }
}
