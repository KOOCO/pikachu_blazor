using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public interface ISetItemDetailsAppService :
    ICrudAppService< 
                SetItemDetailsDto, 
        Guid, 
        PagedAndSortedResultRequestDto,
        CreateUpdateSetItemDetailsDto,
        CreateUpdateSetItemDetailsDto>
{

}