using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public interface IItemDetailsAppService :
    ICrudAppService< 
                ItemDetailsDto, 
        Guid, 
        PagedAndSortedResultRequestDto,
        CreateUpdateItemDetailsDto,
        CreateUpdateItemDetailsDto>
{

}