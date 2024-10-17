using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupBuyProductRankings.Interface;

public interface IGroupBuyProductRankingRepository : IRepository<GroupBuyProductRanking, Guid>
{
    Task<List<GroupBuyProductRanking>> GetListByGroupBuyIdAsync(Guid groupBuyId);
}
