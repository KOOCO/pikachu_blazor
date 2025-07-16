using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Base abstract implementation for shipping strategies
    /// Provides common functionality and enforces Strategy Pattern structure
    /// </summary>
    public abstract class BaseShippingStrategy : DomainService, IShippingStrategy
    {
        protected readonly ILogger<BaseShippingStrategy> Logger;
        
        protected BaseShippingStrategy(ILogger<BaseShippingStrategy> logger)
        {
            Logger = logger;
        }
        
        /// <summary>
        /// The delivery method this strategy handles
        /// </summary>
        public abstract DeliveryMethod DeliveryMethod { get; }
        
        /// <summary>
        /// The logistics provider associated with this strategy
        /// </summary>
        public abstract LogisticProviders LogisticProvider { get; }
        
        /// <summary>
        /// Validates if the shipping strategy can handle the given order
        /// </summary>
        public virtual async Task<bool> CanProcessShippingAsync(Order order)
        {
            if (order == null)
            {
                Logger.LogWarning("Cannot process shipping for null order");
                return false;
            }
            
            if (order.DeliveryMethod != DeliveryMethod)
            {
                Logger.LogWarning("Order delivery method {OrderDeliveryMethod} does not match strategy {StrategyDeliveryMethod}", 
                    order.DeliveryMethod, DeliveryMethod);
                return false;
            }
            
            return await ValidateDeliveryMethodAsync(order);
        }
        
        /// <summary>
        /// Validates delivery method specific requirements
        /// Override in concrete strategies for specific validation
        /// </summary>
        protected virtual Task<bool> ValidateDeliveryMethodAsync(Order order)
        {
            return Task.FromResult(true);
        }
        
        /// <summary>
        /// Calculates shipping cost for the order based on delivery method
        /// </summary>
        public abstract Task<ShippingCostResult> CalculateShippingCostAsync(Order order, ItemStorageTemperature storageTemperature);
        
        /// <summary>
        /// Creates logistics order for the delivery method
        /// </summary>
        public abstract Task<LogisticsCreationResult> CreateLogisticsOrderAsync(Order order, object orderDelivery);
        
        /// <summary>
        /// Prints shipping label for the delivery method
        /// </summary>
        public abstract Task<ShippingLabelResult> PrintShippingLabelAsync(Order order, object orderDelivery);
        
        /// <summary>
        /// Validates shipping data for the delivery method
        /// </summary>
        public abstract Task<ShippingValidationResult> ValidateShippingDataAsync(Order order, object shippingData);
        
        /// <summary>
        /// Updates shipping status for the delivery method
        /// </summary>
        public abstract Task<ShippingStatusUpdateResult> UpdateShippingStatusAsync(Order order, DeliveryStatus newStatus);
        
        /// <summary>
        /// Creates a failure result for shipping cost calculation
        /// </summary>
        protected ShippingCostResult CreateCostFailureResult(string errorMessage)
        {
            Logger.LogError("Shipping cost calculation failed: {ErrorMessage}", errorMessage);
            return new ShippingCostResult
            {
                IsSuccess = false,
                ErrorMessages = { errorMessage }
            };
        }
        
        /// <summary>
        /// Creates a failure result for logistics creation
        /// </summary>
        protected LogisticsCreationResult CreateLogisticsFailureResult(string errorMessage)
        {
            Logger.LogError("Logistics creation failed: {ErrorMessage}", errorMessage);
            return new LogisticsCreationResult
            {
                IsSuccess = false,
                ErrorMessages = { errorMessage }
            };
        }
        
        /// <summary>
        /// Creates a failure result for shipping label generation
        /// </summary>
        protected ShippingLabelResult CreateLabelFailureResult(string errorMessage)
        {
            Logger.LogError("Shipping label generation failed: {ErrorMessage}", errorMessage);
            return new ShippingLabelResult
            {
                IsSuccess = false,
                ErrorMessages = { errorMessage }
            };
        }
        
        /// <summary>
        /// Creates a failure result for shipping validation
        /// </summary>
        protected ShippingValidationResult CreateValidationFailureResult(string errorMessage)
        {
            Logger.LogError("Shipping validation failed: {ErrorMessage}", errorMessage);
            return new ShippingValidationResult
            {
                IsValid = false,
                ErrorMessages = { errorMessage }
            };
        }
        
        /// <summary>
        /// Creates a failure result for status update
        /// </summary>
        protected ShippingStatusUpdateResult CreateStatusUpdateFailureResult(string errorMessage)
        {
            Logger.LogError("Shipping status update failed: {ErrorMessage}", errorMessage);
            return new ShippingStatusUpdateResult
            {
                IsSuccess = false,
                ErrorMessages = { errorMessage }
            };
        }
    }
}