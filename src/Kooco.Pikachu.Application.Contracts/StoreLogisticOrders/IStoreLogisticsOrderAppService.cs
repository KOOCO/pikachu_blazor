using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.StoreLogisticOrders
{
    public interface IStoreLogisticsOrderAppService: IApplicationService
    {
        Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null);

        Task<PrintObtResponse?> GenerateDeliveryNumberForTCatDeliveryAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null);

        Task<PrintOBTB2SResponse?> GenerateDeliveryNumberForTCat711DeliveryAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null);

        Task GenerateDeliveryNumberForSelfPickupAndHomeDeliveryAsync(Guid orderId, Guid orderDeliveryId);

        Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId, DeliveryMethod? deliveryMethod = null);

        Task<EmapApiResponse> GetStoreAsync(string deliveryMethod);

        Task<string> FindStatusAsync();

        Task<string> OnPrintShippingLabel(OrderDto order, OrderDeliveryDto orderDelivery);

        Task<string> OnBatchPrintingShippingLabel(OrderDto order, List<string> allPayLogisticsId);
    }
}
