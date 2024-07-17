using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuyItemGroups;

public interface IGroupBuyItemGroupAppService :
    ICrudAppService<
        GroupBuyItemGroupDto, 
        Guid, 
        PagedAndSortedResultRequestDto, 
        GroupBuyItemGroupCreateUpdateDto
    >
{
}
