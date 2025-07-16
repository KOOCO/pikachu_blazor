using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.OrderTransactions;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.PaymentStrategies
{
    /// <summary>
    /// Core interface for payment processing strategies
    /// Follows Strategy Pattern to eliminate hard-coded payment method handling
    /// </summary>
    public interface IPaymentStrategy
    {
        /// <summary>
        /// The payment method this strategy handles
        /// </summary>
        PaymentMethods PaymentMethod { get; }
        
        /// <summary>
        /// The payment channel associated with this strategy
        /// </summary>
        PaymentChannel PaymentChannel { get; }
        
        /// <summary>
        /// Validates if the payment strategy can process the given order
        /// </summary>
        /// <param name="order">Order to validate</param>
        /// <returns>True if order can be processed, false otherwise</returns>
        Task<bool> CanProcessPaymentAsync(Order order);
        
        /// <summary>
        /// Initiates payment processing for the order
        /// </summary>
        /// <param name="order">Order to process payment for</param>
        /// <param name="paymentGateway">Payment gateway configuration</param>
        /// <param name="clientBackUrl">URL to redirect client after payment</param>
        /// <param name="isInstallments">Whether to use installment payment</param>
        /// <returns>Payment processing result</returns>
        Task<PaymentProcessingResult> ProcessPaymentAsync(Order order, object paymentGateway, string clientBackUrl, bool isInstallments = false);
        
        /// <summary>
        /// Handles payment confirmation/callback
        /// </summary>
        /// <param name="order">Order being paid</param>
        /// <param name="paymentResult">Payment result from gateway</param>
        /// <returns>Confirmation processing result</returns>
        Task<PaymentConfirmationResult> ConfirmPaymentAsync(Order order, object paymentResult);
        
        /// <summary>
        /// Processes refund for the order
        /// </summary>
        /// <param name="order">Order to refund</param>
        /// <param name="refundAmount">Amount to refund</param>
        /// <param name="reason">Refund reason</param>
        /// <returns>Refund processing result</returns>
        Task<RefundProcessingResult> ProcessRefundAsync(Order order, decimal refundAmount, string reason);
    }
}