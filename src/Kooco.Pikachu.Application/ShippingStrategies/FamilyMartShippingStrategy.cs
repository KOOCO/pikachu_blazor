using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Application.ShippingStrategies
{
    /// <summary>
    /// Shipping strategy for FamilyMart convenience store delivery
    /// Handles various FamilyMart delivery methods (C2C, B2C)
    /// </summary>
    public class FamilyMartShippingStrategy : BaseShippingStrategy
    {
        private readonly DeliveryMethod _deliveryMethod;
        
        public FamilyMartShippingStrategy(DeliveryMethod deliveryMethod, ILogger<BaseShippingStrategy> logger) 
            : base(logger)
        {
            _deliveryMethod = deliveryMethod;
        }
        
        public override DeliveryMethod DeliveryMethod => _deliveryMethod;
        
        public override LogisticProviders LogisticProvider => LogisticProviders.FamilyMart;
        
        protected override Task<bool> ValidateDeliveryMethodAsync(Order order)
        {
            // FamilyMart supports multiple delivery methods
            var supportedMethods = new[]
            {
                DeliveryMethod.FamilyMart1,
                DeliveryMethod.FamilyMartC2C
            };
            
            return Task.FromResult(Array.Exists(supportedMethods, method => method == _deliveryMethod));
        }
        
        public override async Task<ShippingCostResult> CalculateShippingCostAsync(Order order, ItemStorageTemperature storageTemperature)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateCostFailureResult($"Cannot process FamilyMart shipping for order {order.Id}");
                }
                
                decimal cost = storageTemperature switch
                {
                    ItemStorageTemperature.Normal => order.DeliveryCostForNormal ?? 60m, // Default FamilyMart cost
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
                Logger.LogError(ex, "FamilyMart shipping cost calculation failed for order {OrderId}", order.Id);
                return CreateCostFailureResult($"FamilyMart shipping cost calculation failed: {ex.Message}");
            }
        }
        
        public override async Task<LogisticsCreationResult> CreateLogisticsOrderAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLogisticsFailureResult($"Cannot create FamilyMart logistics for order {order.Id}");
                }
                
                // Create logistics order specific to FamilyMart
                Logger.LogInformation("Creating FamilyMart logistics order for order {OrderId}", order.Id);
                
                // This would integrate with FamilyMart's logistics API
                var logisticsOrderId = $"FM-{order.OrderNo}-{DateTime.Now:yyyyMMddHHmmss}";
                
                return new LogisticsCreationResult
                {
                    IsSuccess = true,
                    LogisticsOrderId = logisticsOrderId,
                    TrackingNumber = $"FM{DateTime.Now:yyyyMMdd}{order.Id.ToString()[..6]}",
                    ResponseData = new { Provider = "FamilyMart", OrderId = logisticsOrderId }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "FamilyMart logistics creation failed for order {OrderId}", order.Id);
                return CreateLogisticsFailureResult($"FamilyMart logistics creation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingLabelResult> PrintShippingLabelAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLabelFailureResult($"Cannot print FamilyMart label for order {order.Id}");
                }
                
                Logger.LogInformation("Printing FamilyMart shipping label for order {OrderId}", order.Id);
                
                // This would integrate with FamilyMart's label printing API
                return new ShippingLabelResult
                {
                    IsSuccess = true,
                    LabelContent = new byte[0], // Would contain actual label data
                    ContentType = "application/pdf",
                    FileName = $"FM-Label-{order.OrderNo}.pdf"
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "FamilyMart label printing failed for order {OrderId}", order.Id);
                return CreateLabelFailureResult($"FamilyMart label printing failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingValidationResult> ValidateShippingDataAsync(Order order, object shippingData)
        {
            try
            {
                var validationResult = new ShippingValidationResult { IsValid = true };
                
                // Validate FamilyMart specific requirements
                if (string.IsNullOrEmpty(order.RecipientName))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientName"] = ["Receiver name is required for FamilyMart delivery"];
                }
                
                if (string.IsNullOrEmpty(order.RecipientPhone))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientPhone"] = ["Receiver phone is required for FamilyMart delivery"];
                }
                
                // Validate store selection for pickup
                if (_deliveryMethod == DeliveryMethod.FamilyMart1 || _deliveryMethod == DeliveryMethod.FamilyMartC2C)
                {
                    if (string.IsNullOrEmpty(order.StoreId))
                    {
                        validationResult.IsValid = false;
                        validationResult.FieldErrors["CVSStoreID"] = ["Store selection is required for FamilyMart pickup"];
                    }
                }
                
                return validationResult;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "FamilyMart validation failed for order {OrderId}", order.Id);
                return CreateValidationFailureResult($"FamilyMart validation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingStatusUpdateResult> UpdateShippingStatusAsync(Order order, DeliveryStatus newStatus)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateStatusUpdateFailureResult($"Cannot update FamilyMart status for order {order.Id}");
                }
                
                var previousStatus = order.ShippingStatus.ToString();
                
                Logger.LogInformation("Updating FamilyMart shipping status for order {OrderId} from {OldStatus} to {NewStatus}", 
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
                Logger.LogError(ex, "FamilyMart status update failed for order {OrderId}", order.Id);
                return CreateStatusUpdateFailureResult($"FamilyMart status update failed: {ex.Message}");
            }
        }
    }
}