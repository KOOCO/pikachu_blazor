using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.GroupBuys
{
    public interface IGroupBuyAppService : IApplicationService
    {
        Task GroupBuyItemModuleNoReindexingAsync(Guid groupBuyId, GroupBuyModuleType groupBuyModuleType);
        Task<ShippingMethodResponse> GetGroupBuyShippingMethodAsync(Guid id);
        Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input);
        Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds);
        Task<GroupBuyDto> GetAsync(Guid id, bool includeDetails = false);
        Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input);
        Task DeleteAsync(Guid id);
        Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input);
        Task<GroupBuyDto> GetWithDetailsAsync(Guid id);
        Task<List<string>> GetCarouselImagesAsync(Guid id);
        Task<List<List<string>>> GetCarouselImagesModuleWiseAsync(Guid id);
        Task<List<List<string>>> GetBannerImagesModuleWiseAsync(Guid id);
        Task<List<GroupPurchaseOverviewDto>> GetGroupPurchaseOverviewsAsync(Guid groupBuyId);
        Task<List<ImageDto>> GetBannerImagesAsync(Guid id);
        Task<GroupBuyDto> GetForStoreAsync(Guid id);
        Task<GroupBuyItemGroupWithCountDto> GetPagedItemGroupAsync(Guid id, int skipCount);
        Task<List<GroupBuyItemGroupModuleDetailsDto>> GetGroupBuyModulesAsync(Guid groupBuyId);
        Task<List<GroupBuyItemGroupDto>> GetGroupBuyItemGroupsAsync(Guid groupBuyId);
        Task<List<FreebieDto>> GetFreebieForStoreAsync(Guid groupBuyId);
        Task ChangeGroupBuyAvailability(Guid groupBuyId);
        Task<bool> CheckShortCodeForCreate(string shortCode);
        Task<bool> CheckShortCodeForEdit(string shortCode, Guid Id);
        Task<List<GroupBuyDto>> GetGroupBuyByShortCode(string ShortCode);
        Task<GroupBuyDto> GetGroupBuyofTenant(string ShortCode, Guid TenantId);
        Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyReportListAsync(GetGroupBuyReportListDto input);
        Task<GroupBuyDto> GetWithItemGroupsAsync(Guid id);
        Task<GroupBuyItemGroupDto> GetGroupBuyItemGroupAsync(Guid id);
        Task DeleteGroupBuyItemAsync(Guid id, Guid GroupBuyID);
        Task<GroupBuyReportDetailsDto> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null);
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, bool isChinese = false);
        Task<IRemoteStreamContent> GetAttachmentAsync(Guid id, Guid? tenantId, DateTime sendTime, RecurrenceType recurrenceType);
        Task<List<KeyValueDto>> GetGroupBuyLookupAsync();
        Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyTenantReportListAsync(GetGroupBuyReportListDto input);
        Task<GroupBuyReportDetailsDto> GetGroupBuyTenantReportDetailsAsync(Guid id);
        Task<IRemoteStreamContent> GetTenantsListAsExcelFileAsync(Guid id);
        Task<GroupBuyDto> CopyAsync(Guid Id);
        Task<List<KeyValueDto>> GetAllGroupBuyLookupAsync();
        Task UpdateSortOrderAsync(Guid id, List<GroupBuyItemGroupCreateUpdateDto> itemGroups);
        Task<DeliveryTemperatureCostDto> GetTemperatureCostAsync(ItemStorageTemperature itemStorageTemperature);
        Task<Guid?> GetGroupBuyIdAsync(string shortCode);
    }
}
