﻿using Kooco.Pikachu.EnumValues;
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
        Task<ResponseResultDto> CreateStoreLogisticsOrderAsync(Guid orderId, Guid orderDeliveryId);

        Task<PrintObtResponse?> GenerateDeliveryNumberForTCatDeliveryAsync(Guid orderId, Guid orderDeliveryId);

        Task<PrintOBTB2SResponse?> GenerateDeliveryNumberForTCat711DeliveryAsync(Guid orderId, Guid orderDeliveryId);

        Task<ResponseResultDto> CreateHomeDeliveryShipmentOrderAsync(Guid orderId, Guid orderDeliveryId);

        Task<EmapApiResponse> GetStoreAsync(string deliveryMethod);
    }
}
