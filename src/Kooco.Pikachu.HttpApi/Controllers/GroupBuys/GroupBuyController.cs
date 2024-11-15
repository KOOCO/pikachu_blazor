using Asp.Versioning;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Controllers.GroupBuys;

[RemoteService(IsEnabled = true)]
[ControllerName("GroupBuy")]
[Area("app")]
[Route("api/app/group-buy")]
public class GroupBuyController : AbpController, IGroupBuyAppService
{
    private readonly IGroupBuyAppService _groupBuyAppService;
    public GroupBuyController(
   IGroupBuyAppService groupBuyAppService
       )
    {
        _groupBuyAppService = groupBuyAppService;

    }
    [HttpPost]
    public Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input)
    {
        return _groupBuyAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _groupBuyAppService.DeleteAsync(id);
    }

    [HttpDelete("delete-many")]
    public Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds)
    {
        return _groupBuyAppService.DeleteManyGroupBuyItemsAsync(groupBuyIds);
    }

    [HttpGet("{id}")]
    public Task<GroupBuyDto> GetAsync(Guid id, bool includeDetails = false)
    {
        return _groupBuyAppService.GetAsync(id, includeDetails);
    }

    [HttpGet("get-list")]
    public async Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input)
    {
        return await _groupBuyAppService.GetListAsync(input);
    }

    [HttpGet("with-details/{id}")]
    public Task<GroupBuyDto> GetWithDetailsAsync(Guid id)
    {
        return _groupBuyAppService.GetWithDetailsAsync(id);
    }

    [HttpPut("{id}")]
    public Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input)
    {
        return _groupBuyAppService.UpdateAsync(id, input);
    }

    [HttpPut("ChangeGroupBuyAvailability/{id}")]
    public Task ChangeGroupBuyAvailability(Guid id)
    {
        return _groupBuyAppService.ChangeGroupBuyAvailability(id);
    }

    [HttpGet("get-carousel-images/{id}")]
    public Task<List<string>> GetCarouselImagesAsync(Guid id)
    {
        return _groupBuyAppService.GetCarouselImagesAsync(id);
    }

    [HttpGet("get-carousel-images-module-wise/{id}")]
    public Task<List<List<string>>> GetCarouselImagesModuleWiseAsync(Guid id)
    {
        return _groupBuyAppService.GetCarouselImagesModuleWiseAsync(id);
    }

    [HttpGet("get-banner-images/{id}")]
    public Task<List<ImageDto>> GetBannerImagesAsync(Guid id)
    {
        return _groupBuyAppService.GetBannerImagesAsync(id);
    }

    [HttpGet("get-banner-images-module-wise/{id}")]
    public Task<List<List<string>>> GetBannerImagesModuleWiseAsync(Guid id)
    {
        return _groupBuyAppService.GetBannerImagesModuleWiseAsync(id);
    }

    [HttpGet("get-group-purchase-overviews/{groupBuyId}")]
    public Task<List<GroupPurchaseOverviewDto>> GetGroupPurchaseOverviewsAsync(Guid groupBuyId)
    {
        return _groupBuyAppService.GetGroupPurchaseOverviewsAsync(groupBuyId);
    }

    [HttpGet("for-store/{id}")]
    public Task<GroupBuyDto> GetForStoreAsync(Guid id)
    {
        return _groupBuyAppService.GetForStoreAsync(id);
    }

    [HttpGet("freebie-store/{groupBuyId}")]
    public Task<List<FreebieDto>> GetFreebieForStoreAsync(Guid groupBuyId)
    {
        return _groupBuyAppService.GetFreebieForStoreAsync(groupBuyId);
    }

    [HttpGet("check-shortcode-forcreate/{shortCode}")]
    public Task<bool> CheckShortCodeForCreate(string shortCode)
    {
        return _groupBuyAppService.CheckShortCodeForCreate(shortCode);
    }

    [HttpGet("check-shortcode-foredit")]
    public Task<bool> CheckShortCodeForEdit(string shortCode, Guid Id)
    {
        return _groupBuyAppService.CheckShortCodeForEdit(shortCode, Id);
    }

    [HttpGet("get-by-shortcode/{ShortCode}")]
    public Task<List<GroupBuyDto>> GetGroupBuyByShortCode(string ShortCode)
    {
        return _groupBuyAppService.GetGroupBuyByShortCode(ShortCode);
    }

    [HttpGet("get-paged-item-group/{id}/{skipCount}")]
    public Task<GroupBuyItemGroupWithCountDto> GetPagedItemGroupAsync(Guid id, int skipCount)
    {
        return _groupBuyAppService.GetPagedItemGroupAsync(id, skipCount);
    }

    [HttpGet("get-groupBuy-modules/{groupBuyId}")]
    public Task<List<GroupBuyItemGroupModuleDetailsDto>> GetGroupBuyModulesAsync(Guid groupBuyId)
    {
        return _groupBuyAppService.GetGroupBuyModulesAsync(groupBuyId);
    }

    [HttpGet("get-groupbuy-for-tenant")]
    public Task<GroupBuyDto> GetGroupBuyofTenant(string ShortCode, Guid TenantId)
    {
        return _groupBuyAppService.GetGroupBuyofTenant(ShortCode, TenantId);
    }

    [HttpGet("group-buy-report")]
    public Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyReportListAsync(GetGroupBuyReportListDto input)
    {
        return _groupBuyAppService.GetGroupBuyReportListAsync(input);
    }

    [HttpGet("get-with-item-groups/{id}")]
    public Task<GroupBuyDto> GetWithItemGroupsAsync(Guid id)
    {
        return _groupBuyAppService.GetWithItemGroupsAsync(id);
    }

    [HttpGet("get-temperature-cost/{temp}")]
    public Task<DeliveryTemperatureCostDto> GetTemperatureCostAsync(ItemStorageTemperature temp)
    {
        return _groupBuyAppService.GetTemperatureCostAsync(temp);
    }

    [HttpGet("get-groupbuy-item-group")]
    public Task<GroupBuyItemGroupDto> GetGroupBuyItemGroupAsync(Guid id)
    {
        return _groupBuyAppService.GetGroupBuyItemGroupAsync(id);
    }

    [HttpDelete("delete-group-buy-item/{id}/{GroupBuyID}")]
    public Task DeleteGroupBuyItemAsync(Guid id, Guid GroupBuyID)
    {
        return _groupBuyAppService.DeleteGroupBuyItemAsync(id, GroupBuyID);
    }

    [HttpGet("get-groupbuy-report-details/{id}")]
    public Task<GroupBuyReportDetailsDto> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null)
    {
        return _groupBuyAppService.GetGroupBuyReportDetailsAsync(id, startDate, endDate, orderStatus);
    }

    [HttpGet("get-as-excel/{id}")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, bool isChinese = false)
    {
        return _groupBuyAppService.GetListAsExcelFileAsync(id, startDate, endDate);
    }

    [HttpGet("get-attachment/{id}/{tenantId}/{sendTime}/{recurrenceType}")]
    public Task<IRemoteStreamContent> GetAttachmentAsync(Guid id, Guid? tenantId, DateTime sendTime, RecurrenceType recurrenceType)
    {
        return _groupBuyAppService.GetAttachmentAsync(id, tenantId, sendTime, recurrenceType);
    }

    [HttpGet("get-groupbuy-lookup")]
    public Task<List<KeyValueDto>> GetGroupBuyLookupAsync()
    {
        return _groupBuyAppService.GetGroupBuyLookupAsync();
    }

    [HttpGet("get-groupbuy-tenant-report")]
    public Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyTenantReportListAsync(GetGroupBuyReportListDto input)
    {
        return _groupBuyAppService.GetGroupBuyTenantReportListAsync(input);
    }

    [HttpGet("get-groupbuy-tenant-report-details/{id}")]
    public Task<GroupBuyReportDetailsDto> GetGroupBuyTenantReportDetailsAsync(Guid id)
    {
        return _groupBuyAppService.GetGroupBuyTenantReportDetailsAsync(id);
    }

    [HttpGet("get-tenants-list-as-excel/{id}")]
    public Task<IRemoteStreamContent> GetTenantsListAsExcelFileAsync(Guid id)
    {
        return _groupBuyAppService.GetTenantsListAsExcelFileAsync(id);
    }

    [HttpPost("copy-groupbuy/{id}")]
    public Task<GroupBuyDto> CopyAsync(Guid Id)
    {
        return _groupBuyAppService.CopyAsync(Id);
    }

    [HttpGet("get-all-groupbuy-lookup")]
    public Task<List<KeyValueDto>> GetAllGroupBuyLookupAsync()
    {
        return _groupBuyAppService.GetAllGroupBuyLookupAsync();
    }

    [HttpPut("update-sort-order/{id}")]
    public Task UpdateSortOrderAsync(Guid id, List<GroupBuyItemGroupCreateUpdateDto> itemGroups)
    {
        return _groupBuyAppService.UpdateSortOrderAsync(id, itemGroups);
    }
    [HttpGet("get-groupbuy-shipping-method")]
    public Task<ShippingMethodResponse> GetGroupBuyShippingMethodAsync(Guid id)
    {
        return _groupBuyAppService.GetGroupBuyShippingMethodAsync(id);
    }

    [HttpGet("get-groupBuy-ItemGroups/{groupBuyId}")]
    public Task<List<GroupBuyItemGroupDto>> GetGroupBuyItemGroupsAsync(Guid groupBuyId)
    {
        return _groupBuyAppService.GetGroupBuyItemGroupsAsync(groupBuyId);
    }

    [HttpGet("groupbuy-id-by-shortcode/{shortCode}")]
    public Task<Guid?> GetGroupBuyIdAsync(string shortCode)
    {
        return _groupBuyAppService.GetGroupBuyIdAsync(shortCode);
    }

    [HttpGet("reindexing-moduleNo/{groupBuyId}/{groupBuyModuleType}")]
    public Task GroupBuyItemModuleNoReindexingAsync(Guid groupBuyId, GroupBuyModuleType groupBuyModuleType)
    {
        throw new NotImplementedException();
    }
}
