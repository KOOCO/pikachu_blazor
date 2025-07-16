using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.OrderTransactions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.PaymentStrategies
{
    /// <summary>
    /// Base abstract implementation for payment strategies
    /// Provides common functionality and enforces Strategy Pattern structure
    /// </summary>
    public abstract class BasePaymentStrategy : DomainService, IPaymentStrategy
    {
        protected readonly ILogger<BasePaymentStrategy> Logger;
        
        protected BasePaymentStrategy(ILogger<BasePaymentStrategy> logger)
        {
            Logger = logger;
        }
        
        /// <summary>
        /// The payment method this strategy handles
        /// </summary>
        public abstract PaymentMethods PaymentMethod { get; }
        
        /// <summary>
        /// The payment channel associated with this strategy
        /// </summary>
        public abstract PaymentChannel PaymentChannel { get; }
        
        /// <summary>
        /// Validates if the payment strategy can process the given order
        /// </summary>
        public virtual async Task<bool> CanProcessPaymentAsync(Order order)
        {
            if (order == null)
            {
                Logger.LogWarning("Cannot process payment for null order");
                return false;
            }
            
            if (order.TotalAmount <= 0)
            {
                Logger.LogWarning("Cannot process payment for order {OrderId} with amount {Amount}", 
                    order.Id, order.TotalAmount);
                return false;
            }
            
            return await ValidatePaymentMethodAsync(order);
        }
        
        /// <summary>
        /// Validates payment method specific requirements
        /// Override in concrete strategies for specific validation
        /// </summary>
        protected virtual Task<bool> ValidatePaymentMethodAsync(Order order)
        {
            return Task.FromResult(true);
        }
        
        /// <summary>
        /// Initiates payment processing for the order
        /// </summary>
        public abstract Task<PaymentProcessingResult> ProcessPaymentAsync(Order order, object paymentGateway, string clientBackUrl, bool isInstallments = false);
        
        /// <summary>
        /// Handles payment confirmation/callback
        /// </summary>
        public abstract Task<PaymentConfirmationResult> ConfirmPaymentAsync(Order order, object paymentResult);
        
        /// <summary>
        /// Processes refund for the order
        /// </summary>
        public abstract Task<RefundProcessingResult> ProcessRefundAsync(Order order, decimal refundAmount, string reason);
        
        /// <summary>
        /// Creates a failure result for payment processing
        /// </summary>
        protected PaymentProcessingResult CreateFailureResult(string errorMessage)
        {
            Logger.LogError("Payment processing failed: {ErrorMessage}", errorMessage);
            return new PaymentProcessingResult
            {
                IsSuccess = false,
                ErrorMessages = { errorMessage }
            };
        }
        
        /// <summary>
        /// Creates a failure result for payment confirmation
        /// </summary>
        protected PaymentConfirmationResult CreateConfirmationFailureResult(string errorMessage)
        {
            Logger.LogError("Payment confirmation failed: {ErrorMessage}", errorMessage);
            return new PaymentConfirmationResult
            {
                IsSuccess = false,
                ErrorMessages = { errorMessage }
            };
        }
        
        /// <summary>
        /// Creates a failure result for refund processing
        /// </summary>
        protected RefundProcessingResult CreateRefundFailureResult(string errorMessage)
        {
            Logger.LogError("Refund processing failed: {ErrorMessage}", errorMessage);
            return new RefundProcessingResult
            {
                IsSuccess = false,
                ErrorMessages = { errorMessage }
            };
        }
    }
}