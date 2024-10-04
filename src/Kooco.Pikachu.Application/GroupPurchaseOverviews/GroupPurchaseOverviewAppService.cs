using Kooco.Pikachu.GroupPurchaseOverviews.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupPurchaseOverviews;

public class GroupPurchaseOverviewAppService :
    CrudAppService<
        GroupPurchaseOverview,
        GroupPurchaseOverviewDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateGroupPurchaseOverviewDto
    >, IGroupPurchaseOverviewAppService
{
    #region Constructor
    public GroupPurchaseOverviewAppService(
        IRepository<GroupPurchaseOverview, Guid> Repository
    ) 
        : base(Repository)
    {
    }
    #endregion

    #region Methods
    public async Task<GroupPurchaseOverviewDto> CreateGroupPurchaseOverviewAsync(GroupPurchaseOverviewDto groupPurchaseOverview)
    {
        return await CreateAsync(
            ObjectMapper.Map<GroupPurchaseOverviewDto, CreateUpdateGroupPurchaseOverviewDto>(groupPurchaseOverview)
        );
    }
    #endregion
}
