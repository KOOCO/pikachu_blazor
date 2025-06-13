using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ShoppingCredits
{
    [RemoteService(IsEnabled = false)]
    public class ShoppingCreditUsageSettingAppService(IShoppingCreditUsageSettingRepository shoppingCreditUsageSettingRepository,ShoppingCreditUsageSettingManager shoppingCreditUsageSettingManager) : PikachuAppService, IShoppingCreditUsageSettingAppService
    {
        /// <summary>
        /// Retrieves a ShoppingCreditsUsageSetting by its ID
        /// </summary>
        public async Task<ShoppingCreditUsageSettingDto> GetAsync(Guid id)
        {
            var shoppingCreditUsageSetting = await shoppingCreditUsageSettingRepository.GetWithDetailsAsync(id);

            return ObjectMapper.Map<ShoppingCreditUsageSetting, ShoppingCreditUsageSettingDto>(shoppingCreditUsageSetting);
        }

        /// <summary>
        /// Retrieves a ShoppingCreditsUsageSetting by its ID
        /// </summary>
        public async Task<ShoppingCreditUsageSettingDto> GetFirstAsync()
        {
            var shoppingCreditUsageSetting = await shoppingCreditUsageSettingRepository.FirstOrDefaultAsync();
            if (shoppingCreditUsageSetting is not null)
            {
                await shoppingCreditUsageSettingRepository.EnsureCollectionLoadedAsync(shoppingCreditUsageSetting, x => x.SpecificGroupbuys);
                await shoppingCreditUsageSettingRepository.EnsureCollectionLoadedAsync(shoppingCreditUsageSetting, x => x.SpecificProducts);
            }
            return ObjectMapper.Map<ShoppingCreditUsageSetting, ShoppingCreditUsageSettingDto>(shoppingCreditUsageSetting);
        }

        /// <summary>
        /// Creates a new ShoppingCreditsUsageSetting
        /// </summary>
        public async Task<ShoppingCreditUsageSettingDto> CreateAsync(CreateUpdateShoppingCreditUsageSettingDto input)
        {
            var shoppingCreditUsageSetting = await shoppingCreditUsageSettingManager.CreateAsync(input.AllowUsage, input.DeductionMethod, input.UnifiedMaxDeductiblePoints, input.StagedSettings, input.ApplicableItems, input.UsableGroupbuysScope, input.UsableProductsScope, input.ProductIds, input.GroupbuyIds);
            return ObjectMapper.Map<ShoppingCreditUsageSetting, ShoppingCreditUsageSettingDto>(shoppingCreditUsageSetting);
        }

        /// <summary>
        /// Updates an existing ShoppingCreditsUsageSetting
        /// </summary>
        public async Task<ShoppingCreditUsageSettingDto> UpdateAsync(Guid id, CreateUpdateShoppingCreditUsageSettingDto input)
        {
            
            var shoppingCreditUsageSetting = await shoppingCreditUsageSettingManager.UpdateAsync(id,input.AllowUsage, input.DeductionMethod, input.UnifiedMaxDeductiblePoints, input.StagedSettings, input.ApplicableItems, input.UsableGroupbuysScope, input.UsableProductsScope, input.ProductIds, input.GroupbuyIds);

          
            return ObjectMapper.Map<ShoppingCreditUsageSetting, ShoppingCreditUsageSettingDto>(shoppingCreditUsageSetting);
        }

        /// <summary>
        /// Deletes a ShoppingCreditsUsageSetting by its ID
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await shoppingCreditUsageSettingRepository.DeleteAsync(id);
        }

        public async Task<ShoppingCreditUsageSettingByGroupBuyDto> GetFirstByGroupBuyIdAsync(Guid groupBuyId)
        {
            var shoppingCreditUsageSetting = await shoppingCreditUsageSettingRepository.GetFirstByGroupBuyIdAsync(groupBuyId);
            return ObjectMapper.Map<ShoppingCreditUsageSetting, ShoppingCreditUsageSettingByGroupBuyDto>(shoppingCreditUsageSetting);
        }
    }
}