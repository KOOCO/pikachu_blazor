using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Core interface for shipping/delivery processing strategies
    /// Follows Strategy Pattern to eliminate hard-coded delivery method handling
    /// </summary>
    public interface IShippingStrategy
    {
        /// <summary>
        /// The delivery method this strategy handles
        /// </summary>
        DeliveryMethod DeliveryMethod { get; }
        
        /// <summary>
        /// The logistics provider associated with this strategy
        /// </summary>
        LogisticProviders LogisticProvider { get; }
        
        /// <summary>
        /// Validates if the shipping strategy can handle the given order
        /// </summary>
        /// <param name="order">Order to validate</param>
        /// <returns>True if order can be processed, false otherwise</returns>
        Task<bool> CanProcessShippingAsync(Order order);
        
        /// <summary>
        /// Calculates shipping cost for the order based on delivery method
        /// </summary>
        /// <param name="order">Order to calculate shipping for</param>
        /// <param name="storageTemperature">Storage temperature requirement</param>
        /// <returns>Shipping cost calculation result</returns>
        Task<ShippingCostResult> CalculateShippingCostAsync(Order order, ItemStorageTemperature storageTemperature);
        
        /// <summary>
        /// Creates logistics order for the delivery method
        /// </summary>
        /// <param name="order">Order to create logistics for</param>
        /// <param name="orderDelivery">Order delivery information</param>
        /// <returns>Logistics creation result</returns>
        Task<LogisticsCreationResult> CreateLogisticsOrderAsync(Order order, object orderDelivery);
        
        /// <summary>
        /// Prints shipping label for the delivery method
        /// </summary>
        /// <param name="order">Order to print label for</param>
        /// <param name="orderDelivery">Order delivery information</param>
        /// <returns>Shipping label generation result</returns>
        Task<ShippingLabelResult> PrintShippingLabelAsync(Order order, object orderDelivery);
        
        /// <summary>
        /// Validates shipping data for the delivery method
        /// </summary>
        /// <param name="order">Order to validate</param>
        /// <param name="shippingData">Shipping data to validate</param>
        /// <returns>Validation result</returns>
        Task<ShippingValidationResult> ValidateShippingDataAsync(Order order, object shippingData);
        
        /// <summary>
        /// Updates shipping status for the delivery method
        /// </summary>
        /// <param name="order">Order to update</param>
        /// <param name="newStatus">New shipping status</param>
        /// <returns>Status update result</returns>
        Task<ShippingStatusUpdateResult> UpdateShippingStatusAsync(Order order, DeliveryStatus newStatus);
    }
}