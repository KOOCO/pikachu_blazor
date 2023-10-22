using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderAppService: IApplicationService
    {
        Task<OrderDto> GetAsync(Guid id);
        Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input);
        Task<OrderDto> CreateAsync(CreateOrderDto input);
        Task<OrderDto> UpdateAsync(Guid id,CreateOrderDto input);
        Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input);
        Task<OrderDto> GetWithDetailsAsync(Guid id);
        Task AddStoreCommentAsync(Guid id, string comment);
        Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment);
        Task HandlePaymentAsync(PaymentResult paymentResult);
        Task AddCheckMacValueAsync(Guid id, string checkMacValue);
        Task UpdateOrderItemsAsync(Guid id, List<UpdateOrderItemDto> orderItems);
        Task CancelOrderAsync(Guid id);
    }
}
