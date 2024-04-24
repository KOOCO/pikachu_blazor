using Asp.Versioning;
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
    [RemoteService(IsEnabled = false)]
    [ControllerName("StoreLogisticOrders")]
    [Area("app")]
    [Route("api/app/store-logistic-order")]
    public class StoreLogisticOrderController(
    IStoreLogisticsOrderAppService _storeLogisticsOrderAppService
    ) : AbpController, IStoreLogisticsOrderAppService
    {
        [HttpPost]
        public Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId)
        {
            return _storeLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDeliveryId);
        }
        [HttpPost]
        public Task<ResponseResultDto> CreateStoreLogisticsOrderAsync([FromBody] CreateLogisticsOrder input)
        {

            return _storeLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(input);
        }
        [HttpPost]
        public Task<EmapApiResponse> GetStoreAsync(Guid orderId, Guid orderDeliveryId)
        {
            return _storeLogisticsOrderAppService.GetStoreAsync(orderId, orderDeliveryId);
        }
    }
}
