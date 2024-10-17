using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuyProductRankings.Interface;

public interface IGroupBuyProductRankingAppService :
    ICrudAppService<
        GroupBuyProductRankingDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateGroupBuyProductRankingDto
    >
{
    Task<GroupBuyProductRankingDto> CreateGroupBuyProductRankingAsync(GroupBuyProductRankingDto groupBuyProductRanking);

    Task<List<GroupBuyProductRankingDto>> GetListByGroupBuyIdAsync(Guid groupBuyId);
}
