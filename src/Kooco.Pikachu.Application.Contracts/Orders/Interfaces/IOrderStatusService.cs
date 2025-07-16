using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for Order status management operations
    /// Split from IOrderAppService to follow Interface Segregation Principle
    /// </summary>
    public interface IOrderStatusService : IApplicationService
    {
        /// <summary>
        /// Change order status
        /// </summary>
        Task<OrderDto> ChangeOrderStatus(Guid id, ShippingStatus status);

        /// <summary>
        /// Mark order as to be shipped
        /// </summary>
        Task<OrderDto> OrderToBeShipped(Guid id);

        /// <summary>
        /// Mark order as shipped
        /// </summary>
        Task<OrderDto> OrderShipped(Guid id);

        /// <summary>
        /// Mark order as completed
        /// </summary>
        Task<OrderDto> OrderComplete(Guid id);

        /// <summary>
        /// Mark order as closed
        /// </summary>
        Task<OrderDto> OrderClosed(Guid id);

        /// <summary>
        /// Return order
        /// </summary>
        Task ReturnOrderAsync(Guid id);

        /// <summary>
        /// Exchange order
        /// </summary>
        Task ExchangeOrderAsync(Guid id);

        /// <summary>
        /// Change return status
        /// </summary>
        Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus, bool isRefund);
    }
}