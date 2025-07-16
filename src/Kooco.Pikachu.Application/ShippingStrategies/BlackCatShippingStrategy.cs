using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Application.ShippingStrategies
{
    /// <summary>
    /// Shipping strategy for Black Cat (Yamato) delivery service
    /// Handles various Black Cat delivery methods (Normal, Frozen)
    /// </summary>
    public class BlackCatShippingStrategy : BaseShippingStrategy
    {
        private readonly DeliveryMethod _deliveryMethod;
        
        public BlackCatShippingStrategy(DeliveryMethod deliveryMethod, ILogger<BaseShippingStrategy> logger) 
            : base(logger)
        {
            _deliveryMethod = deliveryMethod;
        }
        
        public override DeliveryMethod DeliveryMethod => _deliveryMethod;
        
        public override LogisticProviders LogisticProvider => LogisticProviders.GreenWorldLogistics;
        
        protected override Task<bool> ValidateDeliveryMethodAsync(Order order)
        {
            // Black Cat supports multiple delivery methods
            var supportedMethods = new[]
            {
                DeliveryMethod.BlackCat1,
                DeliveryMethod.BlackCatFrozen
            };
            
            return Task.FromResult(Array.Exists(supportedMethods, method => method == _deliveryMethod));
        }
        
        public override async Task<ShippingCostResult> CalculateShippingCostAsync(Order order, ItemStorageTemperature storageTemperature)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateCostFailureResult($"Cannot process Black Cat shipping for order {order.Id}");
                }
                
                decimal baseCost = _deliveryMethod == DeliveryMethod.BlackCatFrozen ? 150m : 120m;
                
                decimal cost = storageTemperature switch
                {
                    ItemStorageTemperature.Normal => order.DeliveryCostForNormal ?? baseCost,
                    ItemStorageTemperature.Freeze => order.DeliveryCostForFreeze ?? (baseCost + 30m),
                    ItemStorageTemperature.Frozen => order.DeliveryCostForFrozen ?? (baseCost + 50m),
                    _ => baseCost
                };
                
                return new ShippingCostResult
                {
                    IsSuccess = true,
                    Cost = cost,
                    CostBreakdown = new()
                    {
                        ["BaseCost"] = baseCost,
                        ["TemperatureAdjustment"] = cost - baseCost,
                        ["WeightAdjustment"] = 0m // Could be calculated based on order weight
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Black Cat shipping cost calculation failed for order {OrderId}", order.Id);
                return CreateCostFailureResult($"Black Cat shipping cost calculation failed: {ex.Message}");
            }
        }
        
        public override async Task<LogisticsCreationResult> CreateLogisticsOrderAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLogisticsFailureResult($"Cannot create Black Cat logistics for order {order.Id}");
                }
                
                // Create logistics order specific to Black Cat (via Green World)
                Logger.LogInformation("Creating Black Cat logistics order for order {OrderId}", order.Id);
                
                // This would integrate with Green World's logistics API for Black Cat
                var logisticsOrderId = $"BC-{order.OrderNo}-{DateTime.Now:yyyyMMddHHmmss}";
                
                return new LogisticsCreationResult
                {
                    IsSuccess = true,
                    LogisticsOrderId = logisticsOrderId,
                    TrackingNumber = $"BC{DateTime.Now:yyyyMMdd}{order.Id.ToString()[..6]}",
                    ResponseData = new { Provider = "BlackCat", OrderId = logisticsOrderId, Via = "GreenWorld" }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Black Cat logistics creation failed for order {OrderId}", order.Id);
                return CreateLogisticsFailureResult($"Black Cat logistics creation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingLabelResult> PrintShippingLabelAsync(Order order, object orderDelivery)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateLabelFailureResult($"Cannot print Black Cat label for order {order.Id}");
                }
                
                Logger.LogInformation("Printing Black Cat shipping label for order {OrderId}", order.Id);
                
                // This would integrate with Green World's label printing API for Black Cat
                return new ShippingLabelResult
                {
                    IsSuccess = true,
                    LabelContent = new byte[0], // Would contain actual label data
                    ContentType = "application/pdf",
                    FileName = $"BC-Label-{order.OrderNo}.pdf"
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Black Cat label printing failed for order {OrderId}", order.Id);
                return CreateLabelFailureResult($"Black Cat label printing failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingValidationResult> ValidateShippingDataAsync(Order order, object shippingData)
        {
            try
            {
                var validationResult = new ShippingValidationResult { IsValid = true };
                
                // Validate Black Cat specific requirements
                if (string.IsNullOrEmpty(order.RecipientName))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientName"] = ["Receiver name is required for Black Cat delivery"];
                }
                
                if (string.IsNullOrEmpty(order.RecipientPhone))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["RecipientPhone"] = ["Receiver phone is required for Black Cat delivery"];
                }
                
                if (string.IsNullOrEmpty(order.AddressDetails))
                {
                    validationResult.IsValid = false;
                    validationResult.FieldErrors["ReceiverAddress"] = ["Receiver address is required for Black Cat home delivery"];
                }
                
                // Validate frozen delivery requirements
                if (_deliveryMethod == DeliveryMethod.BlackCatFrozen)
                {
                    var hasFrozenItems = order.OrderItems?.Any(item => 
                        item.DeliveryTemperature == ItemStorageTemperature.Frozen) ?? false;
                        
                    if (!hasFrozenItems)
                    {
                        validationResult.WarningMessages.Add("No frozen items found for frozen delivery method");
                    }
                }
                
                return validationResult;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Black Cat validation failed for order {OrderId}", order.Id);
                return CreateValidationFailureResult($"Black Cat validation failed: {ex.Message}");
            }
        }
        
        public override async Task<ShippingStatusUpdateResult> UpdateShippingStatusAsync(Order order, DeliveryStatus newStatus)
        {
            try
            {
                if (!await CanProcessShippingAsync(order))
                {
                    return CreateStatusUpdateFailureResult($"Cannot update Black Cat status for order {order.Id}");
                }
                
                var previousStatus = order.ShippingStatus.ToString();
                
                Logger.LogInformation("Updating Black Cat shipping status for order {OrderId} from {OldStatus} to {NewStatus}", 
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
                Logger.LogError(ex, "Black Cat status update failed for order {OrderId}", order.Id);
                return CreateStatusUpdateFailureResult($"Black Cat status update failed: {ex.Message}");
            }
        }
    }
}