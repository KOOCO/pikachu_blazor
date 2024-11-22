using Kooco.Pikachu.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.OrderDeliveries
{
    public interface IOrderDeliveryAppService : IApplicationService
    {
        Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid Id);
        Task<OrderDeliveryDto> GetDeliveryOrderAsync(Guid Id);
        Task<OrderDeliveryDto> UpdateShippingDetails(Guid id, CreateOrderDto input);
        Task UpdateOrderDeliveryStatus(Guid Id);
        Task ChangeShippingStatus(Guid orderId);
        Task UpdateDeliveredStatus(Guid orderId);
        Task UpdatePickedUpStatus(Guid orderId);
    }
}
