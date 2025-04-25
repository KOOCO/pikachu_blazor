using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditUsageSettingManager : DomainService
    {
        private readonly IShoppingCreditUsageSettingRepository _shoppingCreditUsageSettingRepository;

        public ShoppingCreditUsageSettingManager(IShoppingCreditUsageSettingRepository shoppingCreditUsageSettingRepository)
        {
            _shoppingCreditUsageSettingRepository = shoppingCreditUsageSettingRepository;
        }

        public async Task<ShoppingCreditUsageSetting> CreateAsync(
            bool allowUsage,
            string deductionMethod,
            decimal unifiedMaxDeductiblePoints,
            string stagedSettings,
            string applicableItems,
            string usableGroupbuysScope,
            string usableProductsScope,
            List<Guid> specificProductIds,
            List<Guid> specificGroupbuyIds)
        {
            var shoppingCreditUsageSetting = new ShoppingCreditUsageSetting(
                GuidGenerator.Create(),
                allowUsage,
                deductionMethod,
                unifiedMaxDeductiblePoints,
                stagedSettings,
                applicableItems,
                usableGroupbuysScope,
                usableProductsScope
            );

            if (specificProductIds != null)
            {
                shoppingCreditUsageSetting.AddSpecificProducts(specificProductIds);
            }

            if (specificGroupbuyIds != null)
            {
                shoppingCreditUsageSetting.AddSpecificGroupbuys(specificGroupbuyIds);
            }

            return await _shoppingCreditUsageSettingRepository.InsertAsync(shoppingCreditUsageSetting);
        }

        public async Task<ShoppingCreditUsageSetting> UpdateAsync(
            Guid id,
            bool allowUsage,
            string deductionMethod,
            decimal unifiedMaxDeductiblePoints,
            string stagedSettings,
            string applicableItems,
            string usableGroupbuysScope,
            string usableProductsScope,
            List<Guid> specificProductIds,
            List<Guid> specificGroupbuyIds)
        {
            var shoppingCreditUsageSetting = await _shoppingCreditUsageSettingRepository.GetAsync(id);
            await _shoppingCreditUsageSettingRepository.EnsureCollectionLoadedAsync(shoppingCreditUsageSetting, x => x.SpecificGroupbuys);
            await _shoppingCreditUsageSettingRepository.EnsureCollectionLoadedAsync(shoppingCreditUsageSetting, x => x.SpecificProducts);
            shoppingCreditUsageSetting.SpecificProducts.Clear();
            shoppingCreditUsageSetting.SpecificGroupbuys.Clear();
            shoppingCreditUsageSetting.AllowUsage=allowUsage;
            shoppingCreditUsageSetting.DeductionMethod= deductionMethod;
            shoppingCreditUsageSetting.UnifiedMaxDeductiblePoints= unifiedMaxDeductiblePoints;
            shoppingCreditUsageSetting.StagedSettings= stagedSettings;
            shoppingCreditUsageSetting.ApplicableItems= applicableItems;
            shoppingCreditUsageSetting.UsableGroupbuysScope= usableGroupbuysScope;
            shoppingCreditUsageSetting.UsableProductsScope= usableProductsScope;

            if (specificProductIds != null)
            {
               
                shoppingCreditUsageSetting.UpdateSpecificProducts(specificProductIds);
            }

            if (specificGroupbuyIds != null)
            {
              
                shoppingCreditUsageSetting.UpdateSpecificGroupbuys(specificGroupbuyIds);
            }

            return await _shoppingCreditUsageSettingRepository.UpdateAsync(shoppingCreditUsageSetting);
        }
    }
}