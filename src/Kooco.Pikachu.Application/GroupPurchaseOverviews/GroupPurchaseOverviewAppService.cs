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
    #region Inject
    private readonly IGroupPurchaseOverviewRepository _GroupPurchaseOverviewRepository;
    #endregion

    #region Constructor
    public GroupPurchaseOverviewAppService(
        IRepository<GroupPurchaseOverview, Guid> Repository,
        IGroupPurchaseOverviewRepository GroupPurchaseOverviewRepository
    ) 
        : base(Repository)
    {
        _GroupPurchaseOverviewRepository = GroupPurchaseOverviewRepository;
    }
    #endregion

    #region Methods
    public async Task<GroupPurchaseOverviewDto> CreateGroupPurchaseOverviewAsync(GroupPurchaseOverviewDto groupPurchaseOverview)
    {
        return await CreateAsync(
            ObjectMapper.Map<GroupPurchaseOverviewDto, CreateUpdateGroupPurchaseOverviewDto>(groupPurchaseOverview)
        );
    }

    public async Task<GroupPurchaseOverviewDto> UpdateGroupPurchaseOverviewAsync(GroupPurchaseOverviewDto groupPurchaseOverview)
    {
        return await UpdateAsync(
            groupPurchaseOverview.Id,
            ObjectMapper.Map<GroupPurchaseOverviewDto, CreateUpdateGroupPurchaseOverviewDto>(groupPurchaseOverview)
        );
    }

    public async Task<List<GroupPurchaseOverviewDto>> GetListByGroupBuyIdAsync(Guid groupBuyId)
    {
        return await MapToGetListOutputDtosAsync(
            await _GroupPurchaseOverviewRepository.GetListByGroupBuyIdAsync(groupBuyId)
        );
    }
    #endregion
}
