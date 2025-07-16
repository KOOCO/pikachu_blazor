using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuys.Interfaces
{
    /// <summary>
    /// Service responsible for GroupBuy CRUD operations
    /// Split from IGroupBuyAppService to follow Interface Segregation Principle
    /// </summary>
    public interface IGroupBuyCrudService : IApplicationService
    {
        /// <summary>
        /// Get GroupBuy list with pagination
        /// </summary>
        Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input);

        /// <summary>
        /// Get GroupBuy by ID
        /// </summary>
        Task<GroupBuyDto> GetAsync(Guid id, bool includeDetails = false);

        /// <summary>
        /// Create new GroupBuy
        /// </summary>
        Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input);

        /// <summary>
        /// Update existing GroupBuy
        /// </summary>
        Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input);

        /// <summary>
        /// Delete GroupBuy
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Delete multiple GroupBuy items
        /// </summary>
        Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds);

        /// <summary>
        /// Get GroupBuy with detailed information
        /// </summary>
        Task<GroupBuyDto> GetWithDetailsAsync(Guid id);

        /// <summary>
        /// Get GroupBuy with item groups
        /// </summary>
        Task<GroupBuyDto> GetWithItemGroupsAsync(Guid id);

        /// <summary>
        /// Get GroupBuy for store display
        /// </summary>
        Task<GroupBuyDto> GetForStoreAsync(Guid id);

        /// <summary>
        /// Copy existing GroupBuy
        /// </summary>
        Task<GroupBuyDto> CopyAsync(Guid id);

        /// <summary>
        /// Change GroupBuy availability status
        /// </summary>
        Task ChangeGroupBuyAvailability(Guid groupBuyId);

        /// <summary>
        /// Check if short code is available for new GroupBuy
        /// </summary>
        Task<bool> CheckShortCodeForCreate(string shortCode);

        /// <summary>
        /// Check if short code is available for existing GroupBuy
        /// </summary>
        Task<bool> CheckShortCodeForEdit(string shortCode, Guid id);

        /// <summary>
        /// Get GroupBuy by short code
        /// </summary>
        Task<List<GroupBuyDto>> GetGroupBuyByShortCode(string shortCode);

        /// <summary>
        /// Get GroupBuy of specific tenant by short code
        /// </summary>
        Task<GroupBuyDto> GetGroupBuyofTenant(string shortCode, Guid tenantId);

        /// <summary>
        /// Get GroupBuy ID by short code
        /// </summary>
        Task<Guid?> GetGroupBuyIdAsync(string shortCode);

        /// <summary>
        /// Get GroupBuy lookup data
        /// </summary>
        Task<List<KeyValueDto>> GetGroupBuyLookupAsync();

        /// <summary>
        /// Get all GroupBuy lookup data
        /// </summary>
        Task<List<KeyValueDto>> GetAllGroupBuyLookupAsync();
    }
}