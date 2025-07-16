using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Application.ShippingStrategies
{
    /// <summary>
    /// Shipping strategy for 7-Eleven convenience store delivery
    /// Handles various 7-Eleven delivery methods (C2C, B2C, Frozen)
    /// </summary>
    public class SevenElevenShippingStrategy : BaseShippingStrategy
    {
        private readonly DeliveryMethod _deliveryMethod;
        
        public SevenElevenShippingStrategy(DeliveryMethod deliveryMethod, ILogger<BaseShippingStrategy> logger) 
            : base(logger)
        {
            _deliveryMethod = deliveryMethod;
        }
        
        public override DeliveryMethod DeliveryMethod => _deliveryMethod;
        
        public override LogisticProviders LogisticProvider => LogisticProviders.SevenToEleven;
        
        protected override Task<bool> ValidateDeliveryMethodAsync(Order order)
        {
            // 7-Eleven supports multiple delivery methods
            var supportedMethods = new[]
            {
                DeliveryMethod.SevenToEleven1,
                DeliveryMethod.SevenToElevenC2C,
                DeliveryMethod.SevenToElevenFrozen
            };
            
            return Task.FromResult(Array.Exists(supportedMethods, method => method == _deliveryMethod));
        }
        
        public override async Task<ShippingCostResult> CalculateShippingCostAsync(Order order, ItemStorageTemperature storageTemperature)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateCostFailureResult($"Cannot process 7-Eleven shipping for order {order.Id}");
                }
                
                decimal cost = storageTemperature switch
                {
                    ItemStorageTemperature.Normal => order.DeliveryCostForNormal ?? 60m, // Default 7-Eleven cost
                    ItemStorageTemperature.Freeze => order.DeliveryCostForFreeze ?? 80m, // Freeze cost
                    ItemStorageTemperature.Frozen => order.DeliveryCostForFrozen ?? 100m, // Frozen cost
                    _ => 60m
                };
                
                return new ShippingCostResult
                {
                    IsSuccess = true,
                    Cost = cost,
                    CostBreakdown = new()
                    {
                        ["BaseCost"] = cost,
                        ["TemperatureAdjustment"] = storageTemperature != ItemStorageTemperature.Normal ? 20m : 0m
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "7-Eleven shipping cost calculation failed for order {OrderId}", order.Id);
                return CreateCostFailureResult($"7-Eleven shipping cost calculation failed: {ex.Message}");
            }
        }
        
        public override async Task<LogisticsCreationResult> CreateLogisticsOrderAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLogisticsFailureResult($"Cannot create 7-Eleven logistics for order {order.Id}");
                }
                
                // Create logistics order specific to 7-Eleven
                Logger.LogInformation("Creating 7-Eleven logistics order for order {OrderId}", order.Id);
                
                // This would integrate with 7-Eleven's logistics API
                var logisticsOrderId = $"7E-{order.OrderNo}-{DateTime.Now:yyyyMMddHHmmss}";
                
                return new LogisticsCreationResult
                {
                    IsSuccess = true,
                    LogisticsOrderId = logisticsOrderId,
                    TrackingNumber = $"7E{DateTime.Now:yyyyMMdd}{order.Id.ToString()[..6]}",
                    ResponseData = new { Provider = "7-Eleven", OrderId = logisticsOrderId }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "7-Eleven logistics creation failed for order {OrderId}", order.Id);
                return CreateLogisticsFailureResult($"7-Eleven logistics creation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingLabelResult> PrintShippingLabelAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLabelFailureResult($"Cannot print 7-Eleven label for order {order.Id}");
                }
                
                Logger.LogInformation("Printing 7-Eleven shipping label for order {OrderId}", order.Id);
                
                // This would integrate with 7-Eleven's label printing API
                return new ShippingLabelResult
                {
                    IsSuccess = true,
                    LabelContent = new byte[0], // Would contain actual label data
                    ContentType = "application/pdf",
                    FileName = $"7E-Label-{order.OrderNo}.pdf"
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "7-Eleven label printing failed for order {OrderId}", order.Id);
                return CreateLabelFailureResult($"7-Eleven label printing failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingValidationResult> ValidateShippingDataAsync(Order order, object shippingData)
        {
            try
            {
                var validationResult = new ShippingValidationResult { IsValid = true };
                
                // Validate 7-Eleven specific requirements
                if (string.IsNullOrEmpty(order.RecipientName))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientName"] = ["Receiver name is required for 7-Eleven delivery"];
                }
                
                if (string.IsNullOrEmpty(order.RecipientPhone))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientPhone"] = ["Receiver phone is required for 7-Eleven delivery"];
                }
                
                // Validate store selection for pickup
                if (_deliveryMethod == DeliveryMethod.SevenToEleven1 || _deliveryMethod == DeliveryMethod.SevenToElevenC2C)
                {
                    if (string.IsNullOrEmpty(order.StoreId))
                    {
                        validationResult.IsValid = false;
                        validationResult.FieldErrors["StoreId"] = ["Store selection is required for 7-Eleven pickup"];
                    }
                }
                
                return validationResult;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "7-Eleven validation failed for order {OrderId}", order.Id);
                return CreateValidationFailureResult($"7-Eleven validation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingStatusUpdateResult> UpdateShippingStatusAsync(Order order, DeliveryStatus newStatus)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateStatusUpdateFailureResult($"Cannot update 7-Eleven status for order {order.Id}");
                }
                
                var previousStatus = order.ShippingStatus.ToString();
                
                Logger.LogInformation("Updating 7-Eleven shipping status for order {OrderId} from {OldStatus} to {NewStatus}", 
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
                Logger.LogError(ex, "7-Eleven status update failed for order {OrderId}", order.Id);
                return CreateStatusUpdateFailureResult($"7-Eleven status update failed: {ex.Message}");
            }
        }
    }
}