using Kooco.Pikachu.Application.ShippingStrategies;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Application.ShippingStrategies
{
    /// <summary>
    /// Factory for creating shipping strategies
    /// Follows Factory Pattern to centralize strategy creation and eliminate switch statements
    /// </summary>
    public class ShippingStrategyFactory : IShippingStrategyFactory, ISingletonDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ShippingStrategyFactory> _logger;
        
        // Cache of supported delivery methods and their corresponding logistics providers
        private static readonly Dictionary<DeliveryMethod, LogisticProviders> DeliveryMethodMapping = new()
        {
            // 7-Eleven delivery methods
            { DeliveryMethod.SevenToEleven1, LogisticProviders.SevenToEleven },
            { DeliveryMethod.SevenToElevenC2C, LogisticProviders.SevenToEleven },
            { DeliveryMethod.SevenToElevenFrozen, LogisticProviders.SevenToEleven },
            
            // FamilyMart delivery methods
            { DeliveryMethod.FamilyMart1, LogisticProviders.FamilyMart },
            { DeliveryMethod.FamilyMartC2C, LogisticProviders.FamilyMart },
            
            // Black Cat delivery methods
            { DeliveryMethod.BlackCat1, LogisticProviders.GreenWorldLogistics },
            { DeliveryMethod.BlackCatFrozen, LogisticProviders.GreenWorldLogistics },
            
            // Home delivery and self-pickup
            { DeliveryMethod.HomeDelivery, LogisticProviders.HomeDelivery },
            { DeliveryMethod.SelfPickup, LogisticProviders.HomeDelivery }, // Self-pickup uses internal handling
            
            // TCat delivery methods (can be added later)
            { DeliveryMethod.TCatDeliveryNormal, LogisticProviders.GreenWorldLogistics },
            { DeliveryMethod.TCatDeliveryFreeze, LogisticProviders.GreenWorldLogistics },
            { DeliveryMethod.TCatDeliveryFrozen, LogisticProviders.GreenWorldLogistics },
            
            // Post Office delivery (can be added later)
            { DeliveryMethod.PostOffice, LogisticProviders.PostOffice }
        };
        
        public ShippingStrategyFactory(
            IServiceProvider serviceProvider,
            ILogger<ShippingStrategyFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        /// <summary>
        /// Creates a shipping strategy for the specified delivery method
        /// </summary>
        public IShippingStrategy? CreateStrategy(DeliveryMethod deliveryMethod)
        {
            try
            {
                return deliveryMethod switch
                {
                    // 7-Eleven strategies
                    DeliveryMethod.SevenToEleven1 or 
                    DeliveryMethod.SevenToElevenC2C or 
                    DeliveryMethod.SevenToElevenFrozen => CreateSevenElevenStrategy(deliveryMethod),
                    
                    // FamilyMart strategies
                    DeliveryMethod.FamilyMart1 or 
                    DeliveryMethod.FamilyMartC2C => CreateFamilyMartStrategy(deliveryMethod),
                    
                    // Black Cat strategies
                    DeliveryMethod.BlackCat1 or 
                    DeliveryMethod.BlackCatFrozen => CreateBlackCatStrategy(deliveryMethod),
                    
                    // Home delivery
                    DeliveryMethod.HomeDelivery => CreateHomeDeliveryStrategy(),
                    
                    // Self pickup
                    DeliveryMethod.SelfPickup => CreateSelfPickupStrategy(),
                    
                    // TCat strategies (placeholder for future implementation)
                    DeliveryMethod.TCatDeliveryNormal or 
                    DeliveryMethod.TCatDeliveryFreeze or 
                    DeliveryMethod.TCatDeliveryFrozen => CreateTCatStrategy(deliveryMethod),
                    
                    // Post Office strategy (placeholder for future implementation)
                    DeliveryMethod.PostOffice => CreatePostOfficeStrategy(),
                    
                    _ => null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create shipping strategy for delivery method {DeliveryMethod}", deliveryMethod);
                return null;
            }
        }
        
        /// <summary>
        /// Gets all supported delivery methods
        /// </summary>
        public IEnumerable<DeliveryMethod> GetSupportedDeliveryMethods()
        {
            return DeliveryMethodMapping.Keys.AsEnumerable();
        }
        
        /// <summary>
        /// Checks if a delivery method is supported
        /// </summary>
        public bool IsDeliveryMethodSupported(DeliveryMethod deliveryMethod)
        {
            return DeliveryMethodMapping.ContainsKey(deliveryMethod);
        }
        
        /// <summary>
        /// Gets the logistics provider for a delivery method
        /// </summary>
        public LogisticProviders? GetLogisticProviderForDeliveryMethod(DeliveryMethod deliveryMethod)
        {
            return DeliveryMethodMapping.TryGetValue(deliveryMethod, out var provider) ? provider : null;
        }
        
        /// <summary>
        /// Creates 7-Eleven strategy for various delivery methods
        /// </summary>
        private IShippingStrategy CreateSevenElevenStrategy(DeliveryMethod deliveryMethod)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<BaseShippingStrategy>>();
            return new SevenElevenShippingStrategy(deliveryMethod, logger);
        }
        
        /// <summary>
        /// Creates FamilyMart strategy for various delivery methods
        /// </summary>
        private IShippingStrategy CreateFamilyMartStrategy(DeliveryMethod deliveryMethod)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<BaseShippingStrategy>>();
            return new FamilyMartShippingStrategy(deliveryMethod, logger);
        }
        
        /// <summary>
        /// Creates Black Cat strategy for various delivery methods
        /// </summary>
        private IShippingStrategy CreateBlackCatStrategy(DeliveryMethod deliveryMethod)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<BaseShippingStrategy>>();
            return new BlackCatShippingStrategy(deliveryMethod, logger);
        }
        
        /// <summary>
        /// Creates Home Delivery strategy
        /// </summary>
        private IShippingStrategy CreateHomeDeliveryStrategy()
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<BaseShippingStrategy>>();
            return new HomeDeliveryShippingStrategy(logger);
        }
        
        /// <summary>
        /// Creates Self Pickup strategy
        /// </summary>
        private IShippingStrategy CreateSelfPickupStrategy()
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<BaseShippingStrategy>>();
            return new SelfPickupShippingStrategy(logger);
        }
        
        /// <summary>
        /// Creates TCat strategy for various delivery methods
        /// Placeholder for future implementation
        /// </summary>
        private IShippingStrategy CreateTCatStrategy(DeliveryMethod deliveryMethod)
        {
            // For now, return a placeholder or throw not implemented
            // In future, implement TCatShippingStrategy
            _logger.LogWarning("TCat shipping strategy not yet implemented for {DeliveryMethod}", deliveryMethod);
            return null;
        }
        
        /// <summary>
        /// Creates Post Office strategy
        /// Placeholder for future implementation
        /// </summary>
        private IShippingStrategy CreatePostOfficeStrategy()
        {
            // For now, return a placeholder or throw not implemented
            // In future, implement PostOfficeShippingStrategy
            _logger.LogWarning("Post Office shipping strategy not yet implemented");
            return null;
        }
    }
}