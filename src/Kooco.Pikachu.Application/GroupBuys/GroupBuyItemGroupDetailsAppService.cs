using Kooco.Pikachu.GroupBuyItemGroupsDetails;
using Kooco.Pikachu.Groupbuys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuyItemGroupDetailsAppService :
    CrudAppService<
        GroupBuyItemGroupDetails, 
        GroupBuyItemGroupDetailsDto, 
        Guid, 
        PagedAndSortedResultRequestDto, 
        GroupBuyItemGroupDetailCreateUpdateDto
    >, IGroupBuyItemGroupDetailsAppService
{
    #region Constructor
    public GroupBuyItemGroupDetailsAppService(
        IRepository<GroupBuyItemGroupDetails, Guid> repository
    ) : base(repository)
    {
    }
    #endregion
}
