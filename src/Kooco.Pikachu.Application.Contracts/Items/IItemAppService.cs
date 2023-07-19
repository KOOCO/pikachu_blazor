using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public interface IItemAppService :
    ICrudAppService< 
        ItemDto, 
        Guid, 
        ItemGetListInput,
        CreateUpdateItemDto,
        CreateUpdateItemDto>
{

}