using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc;

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
}
