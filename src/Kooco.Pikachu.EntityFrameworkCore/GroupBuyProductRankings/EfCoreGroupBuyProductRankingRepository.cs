using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.GroupBuyProductRankings.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupBuyProductRankings;

public class EfCoreGroupBuyProductRankingRepository :
    EfCoreRepository<
        PikachuDbContext,
        GroupBuyProductRanking,
        Guid
    >, IGroupBuyProductRankingRepository
{
    #region Constructor
    public EfCoreGroupBuyProductRankingRepository(
        IDbContextProvider<PikachuDbContext> DbContextProvider
    ) 
        : base(DbContextProvider)
    {
    }
    #endregion

    #region Methods
    public async Task<List<GroupBuyProductRanking>> GetListByGroupBuyIdAsync(Guid groupBuyId)
    {
        return [.. (await GetQueryableAsync()).Where(w => w.GroupBuyId == groupBuyId)
                                              .OrderBy(o => o.ModuleNumber)];
    }
    #endregion
}
