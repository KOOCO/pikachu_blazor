using Kooco.Pikachu.GroupBuyItemGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuyItemGroupAppService :
    CrudAppService<
        GroupBuyItemGroup, 
        GroupBuyItemGroupDto, 
        Guid, 
        PagedAndSortedResultRequestDto, 
        GroupBuyItemGroupCreateUpdateDto
    >, IGroupBuyItemGroupAppService
{
    #region Constructor
    public GroupBuyItemGroupAppService(
        IRepository<GroupBuyItemGroup, Guid> repository
    ) : base(repository)
    {
    }
    #endregion
}
