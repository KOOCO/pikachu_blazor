using Kooco.Pikachu.Freebies.Dtos;
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
        Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input);
        Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds);
        Task<GroupBuyDto> GetAsync(Guid id, bool includeDetails = false);
        Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input);
        Task DeleteAsync(Guid id);
        Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input);
        Task<GroupBuyDto> GetWithDetailsAsync(Guid id);
        Task<List<string>> GetCarouselImagesAsync(Guid id);
        Task<GroupBuyDto> GetForStoreAsync(Guid id);
        Task<GroupBuyItemGroupWithCountDto> GetPagedItemGroupAsync(Guid id, int skipCount);
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
        Task<GroupBuyReportDetailsDto> GetGroupBuyReportDetailsAsync(Guid id);
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(Guid id);
    }
}
