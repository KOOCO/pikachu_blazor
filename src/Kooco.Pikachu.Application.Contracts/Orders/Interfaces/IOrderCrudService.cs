using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for Order CRUD operations
    /// Split from IOrderAppService to follow Interface Segregation Principle
    /// </summary>
    public interface IOrderCrudService : IApplicationService
    {
        /// <summary>
        /// Get order by ID
        /// </summary>
        Task<OrderDto> GetAsync(Guid id);

        /// <summary>
        /// Get order with detailed information
        /// </summary>
        Task<OrderDto> GetWithDetailsAsync(Guid id);

        /// <summary>
        /// Get order by group buy and order number
        /// </summary>
        Task<OrderDto> GetOrderAsync(Guid groupBuyId, string orderNo, string extraInfo);

        /// <summary>
        /// Get order list with pagination
        /// </summary>
        Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input, bool hideCredentials = false);

        /// <summary>
        /// Create new order
        /// </summary>
        Task<OrderDto> CreateAsync(CreateUpdateOrderDto input);

        /// <summary>
        /// Update existing order
        /// </summary>
        Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input);

        /// <summary>
        /// Cancel order
        /// </summary>
        Task CancelOrderAsync(Guid id);

        /// <summary>
        /// Merge multiple orders into one
        /// </summary>
        Task<OrderDto> MergeOrdersAsync(List<Guid> ids);

        /// <summary>
        /// Split order into multiple orders
        /// </summary>
        Task<OrderDto> SplitOrderAsync(List<Guid> orderItemIds, Guid orderId);

        /// <summary>
        /// Get order ID by order number
        /// </summary>
        Task<Guid> GetOrderIdAsync(string orderNo);

        /// <summary>
        /// Update orders for enterprise purchase
        /// </summary>
        Task UpdateOrdersIfIsEnterpricePurchaseAsync(Guid groupBuyId);

        /// <summary>
        /// Expire order
        /// </summary>
        Task ExpireOrderAsync(Guid orderId);

        /// <summary>
        /// Close orders automatically
        /// </summary>
        Task CloseOrdersAsync();
    }
}