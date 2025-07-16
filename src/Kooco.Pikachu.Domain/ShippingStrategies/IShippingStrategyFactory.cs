using Kooco.Pikachu.EnumValues;
using System.Collections.Generic;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Factory interface for creating shipping strategies
    /// Follows Factory Pattern to centralize strategy creation logic
    /// </summary>
    public interface IShippingStrategyFactory
    {
        /// <summary>
        /// Creates a shipping strategy for the specified delivery method
        /// </summary>
        /// <param name="deliveryMethod">Delivery method to create strategy for</param>
        /// <returns>Shipping strategy instance, or null if not supported</returns>
        IShippingStrategy? CreateStrategy(DeliveryMethod deliveryMethod);
        
        /// <summary>
        /// Gets all supported delivery methods
        /// </summary>
        /// <returns>Collection of supported delivery methods</returns>
        IEnumerable<DeliveryMethod> GetSupportedDeliveryMethods();
        
        /// <summary>
        /// Checks if a delivery method is supported
        /// </summary>
        /// <param name="deliveryMethod">Delivery method to check</param>
        /// <returns>True if supported, false otherwise</returns>
        bool IsDeliveryMethodSupported(DeliveryMethod deliveryMethod);
        
        /// <summary>
        /// Gets the logistics provider for a delivery method
        /// </summary>
        /// <param name="deliveryMethod">Delivery method to get provider for</param>
        /// <returns>Logistics provider if found, null otherwise</returns>
        LogisticProviders? GetLogisticProviderForDeliveryMethod(DeliveryMethod deliveryMethod);
    }
}