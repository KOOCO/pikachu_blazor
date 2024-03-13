using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.StoreLogisticOrders
{
    public interface IStoreLogisticsOrderAppService: IApplicationService
    {

        Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(CreateLogisticsOrder input);
        Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId);
        Task<EmapApiResponse> GetStoreAsync(Guid orderId, Guid orderDeliveryId);

    }
}
