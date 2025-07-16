using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;

namespace Kooco.Pikachu.PaymentStrategies
{
    /// <summary>
    /// Factory interface for creating payment strategies
    /// Follows Factory Pattern to centralize strategy creation logic
    /// </summary>
    public interface IPaymentStrategyFactory
    {
        /// <summary>
        /// Creates a payment strategy for the specified payment method
        /// </summary>
        /// <param name="paymentMethod">Payment method to create strategy for</param>
        /// <returns>Payment strategy instance, or null if not supported</returns>
        IPaymentStrategy? CreateStrategy(PaymentMethods paymentMethod);
        
        /// <summary>
        /// Gets all supported payment methods
        /// </summary>
        /// <returns>Collection of supported payment methods</returns>
        IEnumerable<PaymentMethods> GetSupportedPaymentMethods();
        
        /// <summary>
        /// Checks if a payment method is supported
        /// </summary>
        /// <param name="paymentMethod">Payment method to check</param>
        /// <returns>True if supported, false otherwise</returns>
        bool IsPaymentMethodSupported(PaymentMethods paymentMethod);
    }
}