using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.PaymentGateways;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

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
        Task<PagedResultDto<OrderDto>> GetReturnListAsync(GetOrderListDto input);
        Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus);
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetOrderListDto input);
        Task<PaymentGatewayDto> GetPaymentGatewayConfigurationsAsync(Guid id);
        Task<OrderDto> MergeOrdersAsync(List<Guid> Ids);
        Task<OrderDto> SplitOrderAsync(List<Guid> OrderItemIds, Guid OrderId);
        Task ExchangeOrderAsync(Guid id);
        Task<PagedResultDto<OrderDto>> GetTenantOrderListAsync(GetOrderListDto input);
    }
}
