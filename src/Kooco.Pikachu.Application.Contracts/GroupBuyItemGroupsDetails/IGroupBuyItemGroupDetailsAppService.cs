using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuyItemGroupsDetails;

public interface IGroupBuyItemGroupDetailsAppService :
    ICrudAppService<
        GroupBuyItemGroupDetailsDto, 
        Guid, 
        PagedAndSortedResultRequestDto, 
        GroupBuyItemGroupDetailCreateUpdateDto
    >
{
}
