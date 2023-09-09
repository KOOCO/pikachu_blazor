using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public interface ISetItemAppService :
    ICrudAppService< 
                SetItemDto, 
        Guid, 
        PagedAndSortedResultRequestDto,
        CreateUpdateSetItemDto,
        CreateUpdateSetItemDto>
{
    Task DeleteManyItemsAsync(List<Guid> setItemIds);
}