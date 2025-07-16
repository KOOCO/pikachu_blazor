using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for order logistics and shipping operations
    /// Extracted from OrderAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IOrderLogisticsService : IApplicationService
    {
        /// <summary>
        /// Update shipping details for an order
        /// </summary>
        Task<OrderDto> UpdateShippingDetails(Guid orderId, CreateOrderDto input);

        /// <summary>
        /// Update logistic status for an order
        /// </summary>
        Task UpdateLogisticStatusAsync(string merchantTradeNo, string rtnMsg, int rtnCode = 0);

        /// <summary>
        /// Mark order as shipped
        /// </summary>
        Task<OrderDto> OrderShipped(Guid orderId);

        /// <summary>
        /// Mark order as completed
        /// </summary>
        Task<OrderDto> OrderComplete(Guid orderId);

        /// <summary>
        /// Mark order as closed
        /// </summary>
        Task<OrderDto> OrderClosed(Guid orderId);

        /// <summary>
        /// Create order deliveries
        /// </summary>
        Task CreateOrderDeliveriesAsync(Guid orderId);

        /// <summary>
        /// Create order deliveries and invoice
        /// </summary>
        Task CreateOrderDeliveriesAndInvoiceAsync(Guid orderId);

        /// <summary>
        /// Process order exchange
        /// </summary>
        Task ExchangeOrderAsync(Guid orderId);

        /// <summary>
        /// Change return status for an order
        /// </summary>
        Task ChangeReturnStatusAsync(Guid orderId, OrderReturnStatus? orderReturnStatus, bool isRefund);
    }
}