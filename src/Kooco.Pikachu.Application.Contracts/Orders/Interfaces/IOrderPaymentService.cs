using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for order payment processing operations
    /// Extracted from OrderAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IOrderPaymentService : IApplicationService
    {
        /// <summary>
        /// Handle payment processing result
        /// </summary>
        Task<string> HandlePaymentAsync(PaymentResult paymentResult);

        /// <summary>
        /// Add payment verification values to order
        /// </summary>
        Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo, PaymentMethods? paymentMethod = null);

        /// <summary>
        /// Add payment verification values to order (overload)
        /// </summary>
        Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo);

        /// <summary>
        /// Add check MAC value for payment verification
        /// </summary>
        Task AddCheckMacValueAsync(Guid id, string checkMacValue);

        /// <summary>
        /// Generate merchant trade number for payment processing
        /// </summary>
        string GenerateMerchantTradeNo(string orderNo);

        /// <summary>
        /// Update order payment method
        /// </summary>
        Task<OrderDto> UpdateOrderPaymentMethodAsync(OrderPaymentMethodRequest request);

        /// <summary>
        /// Update merchant trade number
        /// </summary>
        Task<OrderDto> UpdateMerchantTradeNoAsync(OrderPaymentMethodRequest request);

        /// <summary>
        /// Get payment gateway configurations for an order
        /// </summary>
        Task<PaymentGatewayDto> GetPaymentGatewayConfigurationsAsync(Guid orderId);

        /// <summary>
        /// Process order refund for specific items
        /// </summary>
        Task<OrderDto> RefundOrderItems(List<Guid> OrderItemIds, Guid OrderId);

        /// <summary>
        /// Process payment checkout using Strategy Pattern
        /// Returns payment processing result as string (HTML form or redirect URL)
        /// </summary>
        Task<string> ProceedToCheckoutAsync(Guid orderId, string clientBackUrl, PaymentMethods paymentMethodsValue, bool isInstallments = false);
    }
}