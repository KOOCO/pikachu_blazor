using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuys.Interfaces
{
    /// <summary>
    /// Service responsible for GroupBuy item group operations
    /// Split from IGroupBuyAppService to follow Interface Segregation Principle
    /// </summary>
    public interface IGroupBuyItemGroupService : IApplicationService
    {
        /// <summary>
        /// Get paged item groups for a GroupBuy
        /// </summary>
        Task<GroupBuyItemGroupWithCountDto> GetPagedItemGroupAsync(Guid id, int skipCount);

        /// <summary>
        /// Get GroupBuy modules
        /// </summary>
        Task<List<GroupBuyItemGroupModuleDetailsDto>> GetGroupBuyModulesAsync(Guid groupBuyId);

        /// <summary>
        /// Get all item groups for a GroupBuy
        /// </summary>
        Task<List<GroupBuyItemGroupDto>> GetGroupBuyItemGroupsAsync(Guid groupBuyId);

        /// <summary>
        /// Get specific GroupBuy item group
        /// </summary>
        Task<GroupBuyItemGroupDto> GetGroupBuyItemGroupAsync(Guid id);

        /// <summary>
        /// Delete GroupBuy item
        /// </summary>
        Task DeleteGroupBuyItemAsync(Guid id, Guid groupBuyId);

        /// <summary>
        /// Update sort order of item groups
        /// </summary>
        Task UpdateSortOrderAsync(Guid id, List<GroupBuyItemGroupCreateUpdateDto> itemGroups);

        /// <summary>
        /// Update item product price
        /// </summary>
        Task UpdateItemProductPrice(Guid groupbuyId, GroupBuyItemGroupDetailCreateUpdateDto item);

        /// <summary>
        /// Reindex GroupBuy item module numbers
        /// </summary>
        Task GroupBuyItemModuleNoReindexingAsync(Guid groupBuyId, GroupBuyModuleType groupBuyModuleType);
    }
}