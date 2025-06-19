using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.ProductCategories;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Items;

public interface IItemAppService :
    ICrudAppService< 
        ItemDto, 
        Guid, 
        PagedAndSortedResultRequestDto,
        CreateItemDto,UpdateItemDto>
{
    Task ChangeItemAvailability(Guid itemId);
    Task DeleteManyItemsAsync(List<Guid> itemIds);
    Task<ItemDto> GetAsync(Guid id, bool includeDetails = false);
    Task DeleteSingleImageAsync(Guid itemId, string blobImageName);
    Task<List<ItemWithItemTypeDto>> GetItemsLookupAsync();
    Task<string?> GetFirstImageUrlAsync(Guid id);
    Task<List<ItemDto>> GetListForStoreAsync();
    Task<PagedResultDto<ItemListDto>> GetItemsListAsync(GetItemListDto input);
    Task<List<KeyValueDto>> LookupAsync();
    Task<List<KeyValueDto>> GetAllItemsLookupAsync();
    Task<ItemDto> CopyAysnc(Guid Id);
    Task<ItemDto> GetSKUAndItemAsync(Guid itemId, Guid itemDetailId);
    Task<List<ItemDto>> GetManyAsync(List<Guid> itemIds);
    Task<List<CategoryProductDto>> GetItemCategoriesAsync(Guid id);
    Task<List<ItemDto>> GetItemsWithAttributesAsync(List<Guid> ids);
    Task<List<ItemBadgeDto>> GetItemBadgesAsync();
    Task DeleteItemBadgeAsync(ItemBadgeDto input);
    Task<IRemoteStreamContent> ExportItemListToExcelAsync(List<Guid> itemIds);
    Task ImportItemsFromExcelAsync(IRemoteStreamContent file);
    Task<List<KeyValueDto>> GetItemDetailLookupAsync(Guid itemId);
}