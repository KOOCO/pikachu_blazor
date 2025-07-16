using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for order notification and communication operations
    /// Extracted from OrderAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IOrderNotificationService : IApplicationService
    {
        /// <summary>
        /// Send order confirmation email
        /// </summary>
        Task SendOrderConfirmationEmailAsync(Guid orderId);

        /// <summary>
        /// Send payment confirmation email
        /// </summary>
        Task SendPaymentConfirmationEmailAsync(Guid orderId);

        /// <summary>
        /// Send shipping notification email
        /// </summary>
        Task SendShippingNotificationEmailAsync(Guid orderId);

        /// <summary>
        /// Send order completion email
        /// </summary>
        Task SendOrderCompletionEmailAsync(Guid orderId);

        /// <summary>
        /// Send order cancellation email
        /// </summary>
        Task SendOrderCancellationEmailAsync(Guid orderId);

        /// <summary>
        /// Send refund notification email
        /// </summary>
        Task SendRefundNotificationEmailAsync(Guid orderId);

        /// <summary>
        /// Send general order email notification
        /// </summary>
        Task SendEmailAsync(Guid orderId);
    }
}