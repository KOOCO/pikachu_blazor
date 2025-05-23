﻿using Newtonsoft.Json.Linq;
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
        private readonly IShoppingCreditUsageSettingRepository _shoppingCreditUsageSettingRepository;
        private readonly ShoppingCreditEarnSettingManager _shoppingCreditEarnSettingManager;

        public ShoppingCreditEarnSettingAppService(
            IShoppingCreditEarnSettingRepository shoppingCreditEarnSettingRepository,
            ShoppingCreditEarnSettingManager shoppingCreditEarnSettingManager,
            IShoppingCreditUsageSettingRepository shoppingCreditUsageSettingRepository)
        {
            _shoppingCreditEarnSettingRepository = shoppingCreditEarnSettingRepository;
            _shoppingCreditEarnSettingManager = shoppingCreditEarnSettingManager;
            _shoppingCreditUsageSettingRepository = shoppingCreditUsageSettingRepository;
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

        public async Task<Dictionary<string, object>> GetShoppingCreditSettingsAsync(Guid groupBuyId)
        {
            // Initialize the JSON response as a dictionary
            var response = new Dictionary<string, object>
            {
                ["Cashback"] = null,
                ["Use"] = null
            };

            // Validate GroupBuyId
            if (groupBuyId == Guid.Empty)
            {
                response["Cashback"] = new { Error = "The provided GroupBuyId is invalid." };
                response["Use"] = new { Error = "The provided GroupBuyId is invalid." };
                return response;
            }

            // Fetch settings
            var earnSetting = await _shoppingCreditEarnSettingRepository.FirstOrDefaultAsync();
          

            var usageSetting = await _shoppingCreditUsageSettingRepository.FirstOrDefaultAsync();


            // Populate Cashback section
            if (earnSetting != null)
            {
                await _shoppingCreditEarnSettingRepository.EnsureCollectionLoadedAsync(earnSetting, x => x.SpecificGroupbuys);
                if (!earnSetting.CashbackEnabled)
                {
                    response["Cashback"] = new { Error = "Shopping credit earn setting is disabled." };
                }
                // Validate Group Buy Scope
                if (!earnSetting.SpecificGroupbuys.Any(x => x.GroupbuyId == groupBuyId) && earnSetting.CashbackApplicableGroupbuys == "SpecificGroupbuys")
                {
                    response["Cashback"] = new { Error = "This group buy cannot be used." };
                    return response;
                }

                response["Cashback"] = new
                {
                    Status = earnSetting.CashbackEnabled,
                    UsagePeriodType = earnSetting.CashbackUsagePeriodType,
                    CashbackUnifiedMaxDeductiblePoints = earnSetting.CashbackUnifiedMaxDeductiblePoints,
                    ValidDays = earnSetting.CashbackValidDays,
                    CalculationMethod = earnSetting.CashbackCalculationMethod,
                    StagedSettings = earnSetting.CashbackStagedSettings
                };
            }
            else
            {
                response["Cashback"] = new { Error = "Shopping credit earn setting not found." };
            }

            // Populate Use section
            if (usageSetting != null)
            {
                await _shoppingCreditUsageSettingRepository.EnsureCollectionLoadedAsync(usageSetting, x => x.SpecificGroupbuys);
                if (!usageSetting.AllowUsage)
                {
                    response["Cashback"] = new { Error = "Shopping credit usage setting is disabled." };
                }
                // Validate Group Buy Scope
                if (!usageSetting.SpecificGroupbuys.Any(x => x.GroupbuyId == groupBuyId) && usageSetting.UsableGroupbuysScope == "SpecificGroupbuys")
                {
                    response["Use"] = new { Error = "This group buy cannot be used." };
                    return response;
                }

                response["Use"] = new
                {
                    Status = usageSetting.AllowUsage,
                    DeductionMethod = usageSetting.DeductionMethod,
                    UnifiedMaxDeductiblePoints = usageSetting.UnifiedMaxDeductiblePoints,
                    StagedSettings = usageSetting.StagedSettings,
                    ApplicableItems = usageSetting.ApplicableItems
                };
            }
            else
            {
                response["Use"] = new { Error = "Shopping credit usage setting not found." };
            }

            return response;
        }

    }
}