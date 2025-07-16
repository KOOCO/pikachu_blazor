using Kooco.Pikachu.DeliveryTemperatureCosts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuys.Interfaces
{
    /// <summary>
    /// Service responsible for GroupBuy pricing and cost calculations
    /// Extracted from GroupBuyAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IGroupBuyPricingService : IApplicationService
    {
        /// <summary>
        /// Get shipping method configurations for a GroupBuy
        /// </summary>
        Task<ShippingMethodResponse> GetGroupBuyShippingMethodAsync(Guid groupBuyId);

        /// <summary>
        /// Calculate delivery temperature costs for a GroupBuy
        /// </summary>
        Task<List<DeliveryTemperatureCostDto>> GetDeliveryTemperatureCostsAsync(Guid groupBuyId);

        /// <summary>
        /// Update pricing configuration for a GroupBuy
        /// </summary>
        Task UpdateGroupBuyPricingAsync(Guid groupBuyId, GroupBuyPricingDto pricingDto);

        /// <summary>
        /// Calculate item group pricing
        /// </summary>
        Task<decimal> CalculateItemGroupPriceAsync(Guid groupBuyId, Guid itemGroupId, int quantity);

        /// <summary>
        /// Get pricing details for a specific GroupBuy
        /// </summary>
        Task<GroupBuyPricingDto> GetGroupBuyPricingAsync(Guid groupBuyId);

        /// <summary>
        /// Validate pricing configuration
        /// </summary>
        Task<bool> ValidatePricingConfigurationAsync(Guid groupBuyId);

        /// <summary>
        /// Update bulk pricing rules for a GroupBuy
        /// </summary>
        Task UpdateBulkPricingRulesAsync(Guid groupBuyId, List<BulkPricingRuleDto> rules);
    }
}