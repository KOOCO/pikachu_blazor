using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Main Order application service that combines all order-related operations
    /// Uses Facade pattern to provide unified access to focused services
    /// Follows Interface Segregation Principle by composing smaller interfaces
    /// </summary>
    public interface IOrderAppService : 
        IOrderCrudService,
        IOrderPaymentService,
        IOrderLogisticsService,
        IOrderNotificationService,
        IOrderInvoiceService,
        IOrderInventoryService,
        IOrderReportingService,
        IOrderStatusService,
        IOrderCommentService
    {
        /// <summary>
        /// Refund specific amount for order
        /// </summary>
        Task RefundAmountAsync(double amount, Guid orderId);
    }
}
