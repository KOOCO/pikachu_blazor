using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupPurchaseOverviews.Interface;

public interface IGroupPurchaseOverviewAppService :
    ICrudAppService<
        GroupPurchaseOverviewDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateGroupPurchaseOverviewDto
    >
{
    Task<GroupPurchaseOverviewDto> CreateGroupPurchaseOverviewAsync(GroupPurchaseOverviewDto groupPurchaseOverview);
}
