using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Application.ShippingStrategies
{
    /// <summary>
    /// Shipping strategy for home delivery service
    /// Handles direct home delivery without third-party logistics
    /// </summary>
    public class HomeDeliveryShippingStrategy : BaseShippingStrategy
    {
        public HomeDeliveryShippingStrategy(ILogger<BaseShippingStrategy> logger) 
            : base(logger)
        {
        }
        
        public override DeliveryMethod DeliveryMethod => DeliveryMethod.HomeDelivery;
        
        public override LogisticProviders LogisticProvider => LogisticProviders.GreenWorldLogistics;
        
        protected override Task<bool> ValidateDeliveryMethodAsync(Order order)
        {
            // Home delivery is generally always available
            return Task.FromResult(true);
        }
        
        public override async Task<ShippingCostResult> CalculateShippingCostAsync(Order order, ItemStorageTemperature storageTemperature)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateCostFailureResult($"Cannot process home delivery shipping for order {order.Id}");
                }
                
                decimal baseCost = 100m; // Standard home delivery cost
                
                decimal cost = storageTemperature switch
                {
                    ItemStorageTemperature.Normal => order.DeliveryCostForNormal ?? baseCost,
                    ItemStorageTemperature.Freeze => order.DeliveryCostForFreeze ?? (baseCost + 40m),
                    ItemStorageTemperature.Frozen => order.DeliveryCostForFrozen ?? (baseCost + 60m),
                    _ => baseCost
                };
                
                // Add distance-based cost adjustment
                decimal distanceAdjustment = CalculateDistanceAdjustment(order);
                cost += distanceAdjustment;
                
                return new ShippingCostResult
                {
                    IsSuccess = true,
                    Cost = cost,
                    CostBreakdown = new()
                    {
                        ["BaseCost"] = baseCost,
                        ["TemperatureAdjustment"] = cost - baseCost - distanceAdjustment,
                        ["DistanceAdjustment"] = distanceAdjustment
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Home delivery shipping cost calculation failed for order {OrderId}", order.Id);
                return CreateCostFailureResult($"Home delivery shipping cost calculation failed: {ex.Message}");
            }
        }
        
        public override async Task<LogisticsCreationResult> CreateLogisticsOrderAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLogisticsFailureResult($"Cannot create home delivery logistics for order {order.Id}");
                }
                
                // Create logistics order for home delivery
                Logger.LogInformation("Creating home delivery logistics order for order {OrderId}", order.Id);
                
                var logisticsOrderId = $"HD-{order.OrderNo}-{DateTime.Now:yyyyMMddHHmmss}";
                
                return new LogisticsCreationResult
                {
                    IsSuccess = true,
                    LogisticsOrderId = logisticsOrderId,
                    TrackingNumber = $"HD{DateTime.Now:yyyyMMdd}{order.Id.ToString()[..6]}",
                    ResponseData = new { Provider = "HomeDelivery", OrderId = logisticsOrderId, DeliveryDate = DateTime.Now.AddDays(1) }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Home delivery logistics creation failed for order {OrderId}", order.Id);
                return CreateLogisticsFailureResult($"Home delivery logistics creation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingLabelResult> PrintShippingLabelAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLabelFailureResult($"Cannot print home delivery label for order {order.Id}");
                }
                
                Logger.LogInformation("Printing home delivery shipping label for order {OrderId}", order.Id);
                
                return new ShippingLabelResult
                {
                    IsSuccess = true,
                    LabelContent = new byte[0], // Would contain actual label data
                    ContentType = "application/pdf",
                    FileName = $"HD-Label-{order.OrderNo}.pdf"
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Home delivery label printing failed for order {OrderId}", order.Id);
                return CreateLabelFailureResult($"Home delivery label printing failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingValidationResult> ValidateShippingDataAsync(Order order, object shippingData)
        {
            try
            {
                var validationResult = new ShippingValidationResult { IsValid = true };
                
                // Validate home delivery specific requirements
                if (string.IsNullOrEmpty(order.RecipientName))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientName"] = ["Receiver name is required for home delivery"];
                }
                
                if (string.IsNullOrEmpty(order.RecipientPhone))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientPhone"] = ["Receiver phone is required for home delivery"];
                }
                
                if (string.IsNullOrEmpty(order.AddressDetails))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["AddressDetails"] = ["Complete address is required for home delivery"];
                }
                
                // Validate delivery time preferences
                if (order.ReceivingTime != null)
                {
                    // Add time preference validation logic
                    validationResult.WarningMessages.Add("Delivery time preference will be considered but not guaranteed");
                }
                
                return validationResult;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Home delivery validation failed for order {OrderId}", order.Id);
                return CreateValidationFailureResult($"Home delivery validation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingStatusUpdateResult> UpdateShippingStatusAsync(Order order, DeliveryStatus newStatus)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateStatusUpdateFailureResult($"Cannot update home delivery status for order {order.Id}");
                }
                
                var previousStatus = order.ShippingStatus.ToString();
                
                Logger.LogInformation("Updating home delivery shipping status for order {OrderId} from {OldStatus} to {NewStatus}", 
                    order.Id, previousStatus, newStatus);
                
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
                Logger.LogError(ex, "Home delivery status update failed for order {OrderId}", order.Id);
                return CreateStatusUpdateFailureResult($"Home delivery status update failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Calculate distance-based cost adjustment for home delivery
        /// </summary>
        private decimal CalculateDistanceAdjustment(Order order)
        {
            // This would typically integrate with a mapping service to calculate distance
            // For now, return a placeholder adjustment
            return 0m; // Would be calculated based on delivery address
        }
    }
}