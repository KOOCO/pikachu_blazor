using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.PaymentStrategies;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Application.PaymentStrategies
{
    /// <summary>
    /// Payment strategy for Cash on Delivery
    /// Handles offline payment processing
    /// </summary>
    public class CashOnDeliveryPaymentStrategy : BasePaymentStrategy
    {
        public CashOnDeliveryPaymentStrategy(ILogger<BasePaymentStrategy> logger) 
            : base(logger)
        {
        }
        
        public override PaymentMethods PaymentMethod => PaymentMethods.CashOnDelivery;
        
        public override PaymentChannel PaymentChannel => PaymentChannel.CashOnDelivery;
        
        protected override Task<bool> ValidatePaymentMethodAsync(Order order)
        {
            // Cash on delivery is always valid for positive amounts
            return Task.FromResult(order.TotalAmount > 0);
        }
        
        public override async Task<PaymentProcessingResult> ProcessPaymentAsync(Order order, object paymentGateway, string clientBackUrl, bool isInstallments = false)
        {
            try
            {
                if (!await CanProcessPaymentAsync(order))
                {
                    return CreateFailureResult($"Cannot process Cash on Delivery payment for order {order.Id}");
                }
                
                // Cash on delivery doesn't require immediate payment processing
                // Just mark the order as awaiting payment
                Logger.LogInformation("Cash on Delivery payment initiated for order {OrderId}", order.Id);
                
                return new PaymentProcessingResult
                {
                    IsSuccess = true,
                    TransactionId = $"COD-{order.OrderNo}",
                    ResponseData = new { PaymentMethod = "CashOnDelivery", Status = "AwaitingPayment" },
                    RedirectUrl = clientBackUrl // Redirect back to client
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cash on Delivery payment processing failed for order {OrderId}", order.Id);
                return CreateFailureResult($"Cash on Delivery payment processing failed: {ex.Message}");
            }
        }
        
        public override async Task<PaymentConfirmationResult> ConfirmPaymentAsync(Order order, object paymentResult)
        {
            try
            {
                // Cash on delivery payment is confirmed when delivery is completed
                // This would typically be called by the delivery service
                Logger.LogInformation("Cash on Delivery payment confirmed for order {OrderId}", order.Id);
                
                return new PaymentConfirmationResult
                {
                    IsSuccess = true,
                    TransactionId = $"COD-{order.OrderNo}",
                    ResponseData = paymentResult
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cash on Delivery payment confirmation failed for order {OrderId}", order.Id);
                return CreateConfirmationFailureResult($"Cash on Delivery payment confirmation failed: {ex.Message}");
            }
        }
        
        public override async Task<RefundProcessingResult> ProcessRefundAsync(Order order, decimal refundAmount, string reason)
        {
            try
            {
                // Cash on delivery refunds are processed manually
                Logger.LogInformation("Cash on Delivery refund initiated for order {OrderId}, amount {Amount}", 
                    order.Id, refundAmount);
                
                return new RefundProcessingResult
                {
                    IsSuccess = true,
                    RefundTransactionId = $"COD-REFUND-{order.OrderNo}",
                    OriginalTransactionId = $"COD-{order.OrderNo}",
                    RefundedAmount = refundAmount,
                    ResponseData = new { RefundMethod = "Manual", Status = "ProcessingRefund", Reason = reason }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cash on Delivery refund processing failed for order {OrderId}", order.Id);
                return CreateRefundFailureResult($"Cash on Delivery refund processing failed: {ex.Message}");
            }
        }
    }
}