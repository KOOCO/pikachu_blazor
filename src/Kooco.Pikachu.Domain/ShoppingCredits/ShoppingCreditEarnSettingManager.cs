using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditEarnSettingManager : DomainService
    {
        private readonly IShoppingCreditEarnSettingRepository _shoppingCreditEarnSettingRepository;

        public ShoppingCreditEarnSettingManager(IShoppingCreditEarnSettingRepository shoppingCreditEarnSettingRepository)
        {
            _shoppingCreditEarnSettingRepository = shoppingCreditEarnSettingRepository;
        }

        public async Task<ShoppingCreditEarnSetting> CreateAsync(
            bool registrationBonusEnabled,
            int registrationEarnedPoints,
            string registrationUsagePeriodType,
            int registrationValidDays,
            bool birthdayBonusEnabled,
            int birthdayEarnedPoints,
            string birthdayUsagePeriodType,
            int birthdayValidDays,
            bool cashbackEnabled,
            string cashbackUsagePeriodType,
            int cashbackValidDays,
            string cashbackCalculationMethod,
            decimal cashbackUnifiedMaxDeductiblePoints,
            string stagedSettings,
            string? cashbackApplicableItems,
            string cashbackApplicableGroupbuys,
            string cashbackApplicableProducts,
            List<Guid> specificProductIds,
            List<Guid> specificGroupbuyIds
            )
        {
            var shoppingCreditEarnSetting = new ShoppingCreditEarnSetting(
                GuidGenerator.Create(),
                registrationBonusEnabled,
                registrationEarnedPoints,
                registrationUsagePeriodType,
                registrationValidDays,
                birthdayBonusEnabled,
                birthdayEarnedPoints,
                birthdayUsagePeriodType,
                birthdayValidDays,
                cashbackEnabled,
                cashbackUsagePeriodType,
                cashbackValidDays,
                cashbackCalculationMethod,
                cashbackUnifiedMaxDeductiblePoints,
                stagedSettings,
                cashbackApplicableItems,
                cashbackApplicableGroupbuys,
                cashbackApplicableProducts
            );

            if (specificProductIds != null)
            {
                shoppingCreditEarnSetting.AddSpecificProducts(specificProductIds);
            }

            if (specificGroupbuyIds != null)
            {
                shoppingCreditEarnSetting.AddSpecificGroupbuys(specificGroupbuyIds);
            }

            return await _shoppingCreditEarnSettingRepository.InsertAsync(shoppingCreditEarnSetting);
        }

        public async Task<ShoppingCreditEarnSetting> UpdateAsync(
            Guid id,
            bool registrationBonusEnabled,
            int registrationEarnedPoints,
            string registrationUsagePeriodType,
            int registrationValidDays,
            bool birthdayBonusEnabled,
            int birthdayEarnedPoints,
            string birthdayUsagePeriodType,
            int birthdayValidDays,
            bool cashbackEnabled,
            string cashbackUsagePeriodType,
            int cashbackValidDays,
            string cashbackCalculationMethod,
            decimal cashbackUnifiedMaxDeductiblePoints,
            string stagedSettings,
            string? cashbackApplicableItems,
            string cashbackApplicableGroupbuys,
            string cashbackApplicableProducts,
            List<Guid> specificProductIds,
           List<Guid> specificGroupbuyIds
            )
        {
            var shoppingCreditEarnSetting = await _shoppingCreditEarnSettingRepository.GetAsync(id);

            shoppingCreditEarnSetting.RegistrationBonusEnabled = registrationBonusEnabled;
            shoppingCreditEarnSetting.RegistrationEarnedPoints = registrationEarnedPoints;
            shoppingCreditEarnSetting.RegistrationUsagePeriodType = registrationUsagePeriodType;
            shoppingCreditEarnSetting.RegistrationValidDays = registrationValidDays;
            shoppingCreditEarnSetting.BirthdayBonusEnabled = birthdayBonusEnabled;
            shoppingCreditEarnSetting.BirthdayEarnedPoints = birthdayEarnedPoints;
            shoppingCreditEarnSetting.BirthdayUsagePeriodType = birthdayUsagePeriodType;
            shoppingCreditEarnSetting.BirthdayValidDays = birthdayValidDays;
            shoppingCreditEarnSetting.CashbackEnabled = cashbackEnabled;
            shoppingCreditEarnSetting.CashbackUsagePeriodType = cashbackUsagePeriodType;
            shoppingCreditEarnSetting.CashbackValidDays = cashbackValidDays;
            shoppingCreditEarnSetting.CashbackCalculationMethod = cashbackCalculationMethod;
            shoppingCreditEarnSetting.CashbackUnifiedMaxDeductiblePoints = cashbackUnifiedMaxDeductiblePoints;
            shoppingCreditEarnSetting.CashbackStagedSettings = stagedSettings;
            shoppingCreditEarnSetting.CashbackApplicableItems = cashbackApplicableItems;
            shoppingCreditEarnSetting.CashbackApplicableGroupbuys = cashbackApplicableGroupbuys;
            shoppingCreditEarnSetting.CashbackApplicableProducts = cashbackApplicableProducts;

            if (specificProductIds != null)
            {
                shoppingCreditEarnSetting.UpdateSpecificProducts(specificProductIds);
            }

            if (specificGroupbuyIds != null)
            {
                shoppingCreditEarnSetting.UpdateSpecificGroupbuys(specificGroupbuyIds);
            }

            return await _shoppingCreditEarnSettingRepository.UpdateAsync(shoppingCreditEarnSetting);
        }
    }
}