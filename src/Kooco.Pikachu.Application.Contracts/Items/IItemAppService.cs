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
    Task DeleteManyItems(List<Guid> itemIds);

    Task DeleteAllAsync();
}