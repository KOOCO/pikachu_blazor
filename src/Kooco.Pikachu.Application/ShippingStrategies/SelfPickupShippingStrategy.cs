using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Application.ShippingStrategies
{
    /// <summary>
    /// Shipping strategy for self-pickup service
    /// Handles customer pickup from store location
    /// </summary>
    public class SelfPickupShippingStrategy : BaseShippingStrategy
    {
        public SelfPickupShippingStrategy(ILogger<BaseShippingStrategy> logger) 
            : base(logger)
        {
        }
        
        public override DeliveryMethod DeliveryMethod => DeliveryMethod.SelfPickup;
        
        public override LogisticProviders LogisticProvider => LogisticProviders.HomeDelivery; // Self-pickup uses internal handling
        
        protected override Task<bool> ValidateDeliveryMethodAsync(Order order)
        {
            // Self-pickup is generally always available
            return Task.FromResult(true);
        }
        
        public override async Task<ShippingCostResult> CalculateShippingCostAsync(Order order, ItemStorageTemperature storageTemperature)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateCostFailureResult($"Cannot process self-pickup shipping for order {order.Id}");
                }
                
                // Self-pickup typically has no delivery cost
                decimal cost = 0m;
                
                // However, there might be special handling fees for temperature-controlled items
                decimal handlingFee = storageTemperature switch
                {
                    ItemStorageTemperature.Freeze => 10m, // Small fee for freeze handling
                    ItemStorageTemperature.Frozen => 15m, // Small fee for frozen handling
                    _ => 0m
                };
                
                cost += handlingFee;
                
                return new ShippingCostResult
                {
                    IsSuccess = true,
                    Cost = cost,
                    CostBreakdown = new()
                    {
                        ["BaseCost"] = 0m,
                        ["TemperatureHandlingFee"] = handlingFee
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Self-pickup shipping cost calculation failed for order {OrderId}", order.Id);
                return CreateCostFailureResult($"Self-pickup shipping cost calculation failed: {ex.Message}");
            }
        }
        
        public override async Task<LogisticsCreationResult> CreateLogisticsOrderAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLogisticsFailureResult($"Cannot create self-pickup logistics for order {order.Id}");
                }
                
                // Create logistics order for self-pickup
                Logger.LogInformation("Creating self-pickup logistics order for order {OrderId}", order.Id);
                
                var logisticsOrderId = $"SP-{order.OrderNo}-{DateTime.Now:yyyyMMddHHmmss}";
                
                return new LogisticsCreationResult
                {
                    IsSuccess = true,
                    LogisticsOrderId = logisticsOrderId,
                    TrackingNumber = $"SP{DateTime.Now:yyyyMMdd}{order.Id.ToString()[..6]}",
                    ResponseData = new { 
                        Provider = "SelfPickup", 
                        OrderId = logisticsOrderId, 
                        PickupLocation = "Store Location",
                        ReadyForPickup = DateTime.Now.AddHours(2) // Ready in 2 hours
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Self-pickup logistics creation failed for order {OrderId}", order.Id);
                return CreateLogisticsFailureResult($"Self-pickup logistics creation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingLabelResult> PrintShippingLabelAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLabelFailureResult($"Cannot print self-pickup label for order {order.Id}");
                }
                
                Logger.LogInformation("Printing self-pickup label for order {OrderId}", order.Id);
                
                // Self-pickup might generate a pickup receipt or order summary
                return new ShippingLabelResult
                {
                    IsSuccess = true,
                    LabelContent = new byte[0], // Would contain pickup receipt data
                    ContentType = "application/pdf",
                    FileName = $"SP-Receipt-{order.OrderNo}.pdf"
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Self-pickup label printing failed for order {OrderId}", order.Id);
                return CreateLabelFailureResult($"Self-pickup label printing failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingValidationResult> ValidateShippingDataAsync(Order order, object shippingData)
        {
            try
            {
                var validationResult = new ShippingValidationResult { IsValid = true };
                
                // Validate self-pickup specific requirements
                if (string.IsNullOrEmpty(order.RecipientName))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientName"] = ["Receiver name is required for self-pickup"];
                }
                
                if (string.IsNullOrEmpty(order.RecipientPhone))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientPhone"] = ["Receiver phone is required for pickup notification"];
                }
                
                // Validate pickup time preferences
                if (order.ReceivingTime != null)
                {
                    // Add pickup time validation logic
                    validationResult.WarningMessages.Add("Pickup time preference noted - please call to confirm availability");
                }
                
                // Check store operating hours
                var now = DateTime.Now;
                if (now.Hour < 9 || now.Hour > 18)
                {
                    validationResult.WarningMessages.Add("Store pickup is available during business hours (9 AM - 6 PM)");
                }
                
                return validationResult;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Self-pickup validation failed for order {OrderId}", order.Id);
                return CreateValidationFailureResult($"Self-pickup validation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingStatusUpdateResult> UpdateShippingStatusAsync(Order order, DeliveryStatus newStatus)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateStatusUpdateFailureResult($"Cannot update self-pickup status for order {order.Id}");
                }
                
                var previousStatus = order.ShippingStatus.ToString();
                
                Logger.LogInformation("Updating self-pickup shipping status for order {OrderId} from {OldStatus} to {NewStatus}", 
                    order.Id, previousStatus, newStatus);
                
                // For self-pickup, certain statuses have special meaning
                if (newStatus == DeliveryStatus.Shipped)
                {
                    // For self-pickup, "shipped" means "ready for pickup"
                    Logger.LogInformation("Order {OrderId} is ready for pickup", order.Id);
                }
                else if (newStatus == DeliveryStatus.Delivered)
                {
                    // For self-pickup, "delivered" means "picked up"
                    Logger.LogInformation("Order {OrderId} has been picked up", order.Id);
                }
                
                return new ShippingStatusUpdateResult
                {
                    IsSuccess = true,
                    PreviousStatus = previousStatus,
                    NewStatus = newStatus.ToString(),
                    UpdatedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Self-pickup status update failed for order {OrderId}", order.Id);
                return CreateStatusUpdateFailureResult($"Self-pickup status update failed: {ex.Message}");
            }
        }
    }
}