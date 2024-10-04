using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ShoppingCredits
{
    [RemoteService(IsEnabled = false)]
    public class ShoppingCreditEarnSettingAppService : PikachuAppService, IShoppingCreditEarnSettingAppService
    {
        private readonly IShoppingCreditEarnSettingRepository _shoppingCreditEarnSettingRepository;
        private readonly ShoppingCreditEarnSettingManager _shoppingCreditEarnSettingManager;

        public ShoppingCreditEarnSettingAppService(
            IShoppingCreditEarnSettingRepository shoppingCreditEarnSettingRepository,
            ShoppingCreditEarnSettingManager shoppingCreditEarnSettingManager)
        {
            _shoppingCreditEarnSettingRepository = shoppingCreditEarnSettingRepository;
            _shoppingCreditEarnSettingManager = shoppingCreditEarnSettingManager;
        }

        /// <summary>
        /// Retrieves a ShoppingCreditsEarnSetting by its ID
        /// </summary>
        public async Task<ShoppingCreditEarnSettingDto> GetAsync(Guid id)
        {
            var shoppingCreditsEarnSetting = await _shoppingCreditEarnSettingRepository.GetWithDetailsAsync(id);
            return ObjectMapper.Map<ShoppingCreditEarnSetting, ShoppingCreditEarnSettingDto>(shoppingCreditsEarnSetting);
        }

        /// <summary>
        /// Retrieves the first ShoppingCreditsEarnSetting (if exists)
        /// </summary>
        public async Task<ShoppingCreditEarnSettingDto> GetFirstAsync()
        {
            var shoppingCreditsEarnSetting = await _shoppingCreditEarnSettingRepository.FirstOrDefaultAsync();
            if (shoppingCreditsEarnSetting != null)
            {
                await _shoppingCreditEarnSettingRepository.EnsureCollectionLoadedAsync(shoppingCreditsEarnSetting, x => x.SpecificProducts);
                await _shoppingCreditEarnSettingRepository.EnsureCollectionLoadedAsync(shoppingCreditsEarnSetting, x => x.SpecificGroupbuys);
            }

            return ObjectMapper.Map<ShoppingCreditEarnSetting, ShoppingCreditEarnSettingDto>(shoppingCreditsEarnSetting);
        }

        /// <summary>
        /// Creates a new ShoppingCreditsEarnSetting
        /// </summary>
        public async Task<ShoppingCreditEarnSettingDto> CreateAsync(CreateUpdateShoppingCreditEarnSettingDto input)
        {
            var shoppingCreditsEarnSetting = await _shoppingCreditEarnSettingManager.CreateAsync(
                input.RegistrationBonusEnabled,
                input.RegistrationEarnedPoints,
                input.RegistrationUsagePeriodType,
                input.RegistrationValidDays,
                input.BirthdayBonusEnabled,
                input.BirthdayEarnedPoints,
                input.BirthdayUsagePeriodType,
                input.BirthdayValidDays,
                input.CashbackEnabled,
                input.CashbackUsagePeriodType,
                input.CashbackValidDays,
                input.CashbackCalculationMethod,
                input.CashbackUnifiedMaxDeductiblePoints,
                input.CashbackStagedSettings,
                input.CashbackApplicableItems,
                input.CashbackApplicableGroupbuys,
                input.CashbackApplicableProducts,
                input.SpecificProductIds,
                input.SpecificGroupbuyIds
            );

            return ObjectMapper.Map<ShoppingCreditEarnSetting, ShoppingCreditEarnSettingDto>(shoppingCreditsEarnSetting);
        }

        /// <summary>
        /// Updates an existing ShoppingCreditsEarnSetting
        /// </summary>
        public async Task<ShoppingCreditEarnSettingDto> UpdateAsync(Guid id, CreateUpdateShoppingCreditEarnSettingDto input)
        {
            var shoppingCreditsEarnSetting = await _shoppingCreditEarnSettingManager.UpdateAsync(
                id,
                input.RegistrationBonusEnabled,
                input.RegistrationEarnedPoints,
                input.RegistrationUsagePeriodType,
                input.RegistrationValidDays,
                input.BirthdayBonusEnabled,
                input.BirthdayEarnedPoints,
                input.BirthdayUsagePeriodType,
                input.BirthdayValidDays,
                input.CashbackEnabled,
                input.CashbackUsagePeriodType,
                input.CashbackValidDays,
                input.CashbackCalculationMethod,
                input.CashbackUnifiedMaxDeductiblePoints,
                input.CashbackStagedSettings,
                input.CashbackApplicableItems,
                input.CashbackApplicableGroupbuys,
                input.CashbackApplicableProducts,
                input.SpecificProductIds,
                input.SpecificGroupbuyIds
            );

            return ObjectMapper.Map<ShoppingCreditEarnSetting, ShoppingCreditEarnSettingDto>(shoppingCreditsEarnSetting);
        }

        
    }
}