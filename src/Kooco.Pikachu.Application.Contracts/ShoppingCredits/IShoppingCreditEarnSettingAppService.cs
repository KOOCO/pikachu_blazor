using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ShoppingCredits
{
    public interface IShoppingCreditEarnSettingAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves a ShoppingCreditsEarnSetting by its ID
        /// </summary>
        /// <param name="id">The ID of the ShoppingCreditsEarnSetting</param>
        /// <returns>ShoppingCreditsEarnSettingDto</returns>
        Task<ShoppingCreditEarnSettingDto> GetAsync(Guid id);

        /// <summary>
        /// Creates a new ShoppingCreditsEarnSetting
        /// </summary>
        /// <param name="input">The details for creating a new ShoppingCreditsEarnSetting</param>
        /// <returns>The created ShoppingCreditsEarnSettingDto</returns>
        Task<ShoppingCreditEarnSettingDto> CreateAsync(CreateUpdateShoppingCreditEarnSettingDto input);

        /// <summary>
        /// Updates an existing ShoppingCreditsEarnSetting
        /// </summary>
        /// <param name="id">The ID of the ShoppingCreditsEarnSetting to update</param>
        /// <param name="input">The updated details of the ShoppingCreditsEarnSetting</param>
        /// <returns>The updated ShoppingCreditsEarnSettingDto</returns>
        Task<ShoppingCreditEarnSettingDto> UpdateAsync(Guid id, CreateUpdateShoppingCreditEarnSettingDto input);

        /// <summary>
        /// Retrieves the first ShoppingCreditsEarnSetting
        /// </summary>
        /// <returns>ShoppingCreditsEarnSettingDto</returns>
        Task<ShoppingCreditEarnSettingDto> GetFirstAsync();

        /// <summary>
        /// Deletes a ShoppingCreditsEarnSetting by its ID
        /// </summary>
        /// <param name="id">The ID of the ShoppingCreditsEarnSetting to delete</param>
        // Task DeleteAsync(Guid id);
    }
}
