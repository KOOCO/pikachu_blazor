using Asp.Versioning;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.ProductCategories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Controllers.Items;

[RemoteService(IsEnabled = true)]
[ControllerName("Item")]
[Area("app")]
[Route("api/app/item")]
public class ItemController(
    IItemAppService _itemAppService
) : AbpController, IItemAppService
{
    [HttpPost("change-availability")]
    public Task ChangeItemAvailability(Guid itemId)
    {
        return _itemAppService.ChangeItemAvailability(itemId);
    }

    [HttpPost]
    public Task<ItemDto> CreateAsync(CreateItemDto input)
    {
        return _itemAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _itemAppService.DeleteAsync(id);
    }

    [HttpDelete("delete-many")]
    public Task DeleteManyItemsAsync(List<Guid> itemIds)
    {
        return _itemAppService.DeleteManyItemsAsync(itemIds);
    }

    [HttpDelete("delete-single-image")]
    public Task DeleteSingleImageAsync(Guid itemId, string blobImageName)
    {
        return _itemAppService.DeleteSingleImageAsync(itemId, blobImageName);
    }

    [HttpGet("{id}")]
    public Task<ItemDto> GetAsync(Guid id, bool includeDetails = false)
    {
        return _itemAppService.GetAsync(id, includeDetails);
    }

    [HttpGet("get-single")]
    public Task<ItemDto> GetAsync(Guid id)
    {
        return ((IReadOnlyAppService<ItemDto, ItemDto, Guid, PagedAndSortedResultRequestDto>)_itemAppService).GetAsync(id);
    }

    [HttpGet("get-first-image-url/{id}")]
    public Task<string?> GetFirstImageUrlAsync(Guid id)
    {
        return _itemAppService.GetFirstImageUrlAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ItemDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _itemAppService.GetListAsync(input);
    }

    [HttpGet("get-list-for-store")]
    public Task<List<ItemDto>> GetListForStoreAsync()
    {
        return _itemAppService.GetListForStoreAsync();
    }

    [HttpPut("{id}")]
    public Task<ItemDto> UpdateAsync(Guid id, UpdateItemDto input)
    {
        return _itemAppService.UpdateAsync(id, input);
    }

    [HttpGet("get-items-lookup")]
    public Task<List<ItemWithItemTypeDto>> GetItemsLookupAsync()
    {
        return _itemAppService.GetItemsLookupAsync();
    }

    [HttpGet("get-items-list")]
    public Task<PagedResultDto<ItemListDto>> GetItemsListAsync(GetItemListDto input)
    {
        return _itemAppService.GetItemsListAsync(input);
    }

    //[HttpGet("get-all-items-lookup")]
    //public Task<List<KeyValueDto>> GetAllItemsLookupAsync()
    //{
    //    return _itemAppService.GetAllItemsLookupAsync();
    //}
    [HttpPost("copy-item/{id}")]
    public Task<ItemDto> CopyAysnc(Guid Id)
    {
        return _itemAppService.CopyAysnc(Id);
    }
    [HttpGet("get-all-items-lookup")]
    public Task<List<KeyValueDto>> GetAllItemsLookupAsync()
    {
        return _itemAppService.GetAllItemsLookupAsync();
    }

    [HttpGet("get-sku-and-item")]
    public Task<ItemDto> GetSKUAndItemAsync(Guid itemId, Guid itemDetailId)
    {
        return _itemAppService.GetSKUAndItemAsync(itemId, itemDetailId);
    }

    [HttpGet("get-many")]
    public Task<List<ItemDto>> GetManyAsync(List<Guid> itemIds)
    {
        return _itemAppService.GetManyAsync(itemIds);
    }

    [HttpGet("item-categories")]
    public Task<List<CategoryProductDto>> GetItemCategoriesAsync(Guid id)
    {
        return _itemAppService.GetItemCategoriesAsync(id);
    }

    [HttpGet("items-with-attributes")]
    public Task<List<ItemDto>> GetItemsWithAttributesAsync(List<Guid> ids)
    {
        return _itemAppService.GetItemsWithAttributesAsync(ids);
    }

    [HttpGet("get-item-badges")]
    public Task<List<ItemBadgeDto>> GetItemBadgesAsync()
    {
        return _itemAppService.GetItemBadgesAsync();
    }

    [HttpDelete("item-badge")]
    public Task DeleteItemBadgeAsync(ItemBadgeDto input)
    {
        return _itemAppService.DeleteItemBadgeAsync(input);
    }

    [HttpGet("lookup")]
    public Task<List<KeyValueDto>> LookupAsync()
    {
        return _itemAppService.LookupAsync();
    }
    [HttpGet("export-excel")]
    public Task<IRemoteStreamContent> ExportItemListToExcelAsync(List<Guid> itemIds)
    {
        return _itemAppService.ExportItemListToExcelAsync(itemIds);
    }
    [HttpPost("import-items")]
    public Task ImportItemsFromExcelAsync(IRemoteStreamContent file)
    {
        return _itemAppService.ImportItemsFromExcelAsync(file);
    }

    [HttpGet("item-detail-lookup/{itemId}")]
    public Task<List<KeyValueDto>> GetItemDetailLookupAsync(Guid itemId)
    {
        return _itemAppService.GetItemDetailLookupAsync(itemId);
    }
}
