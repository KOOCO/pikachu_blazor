using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ShoppingCredits
{
    public interface IShoppingCreditUsageSettingAppService : IApplicationService
    {
       
        /// <summary>
        /// Retrieves a ShoppingCreditsUsageSetting by its ID
        /// </summary>
        /// <param name="id">The ID of the ShoppingCreditsUsageSetting</param>
        /// <returns>ShoppingCreditsUsageSettingDto</returns>
        Task<ShoppingCreditUsageSettingDto> GetAsync(Guid id);

        /// <summary>
        /// Creates a new ShoppingCreditsUsageSetting
        /// </summary>
        /// <param name="input">The details for creating a new ShoppingCreditsUsageSetting</param>
        /// <returns>The created ShoppingCreditsUsageSettingDto</returns>
        Task<ShoppingCreditUsageSettingDto> CreateAsync(CreateUpdateShoppingCreditUsageSettingDto input);

        /// <summary>
        /// Updates an existing ShoppingCreditsUsageSetting
        /// </summary>
        /// <param name="id">The ID of the ShoppingCreditsUsageSetting to update</param>
        /// <param name="input">The updated details of the ShoppingCreditsUsageSetting</param>
        /// <returns>The updated ShoppingCreditsUsageSettingDto</returns>
        Task<ShoppingCreditUsageSettingDto> UpdateAsync(Guid id, CreateUpdateShoppingCreditUsageSettingDto input);
        Task<ShoppingCreditUsageSettingDto> GetFirstAsync();
        /// <summary>
        /// Deletes a ShoppingCreditsUsageSetting by its ID
        /// </summary>
        /// <param name="id">The ID of the ShoppingCreditsUsageSetting to delete</param>
        //Task DeleteAsync(Guid id);
    }
}