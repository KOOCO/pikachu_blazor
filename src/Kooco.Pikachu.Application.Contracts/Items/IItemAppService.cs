using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

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
    Task<List<KeyValueDto>> GetAllItemsLookupAsync();
    Task<ItemDto> CopyAysnc(Guid Id);
}