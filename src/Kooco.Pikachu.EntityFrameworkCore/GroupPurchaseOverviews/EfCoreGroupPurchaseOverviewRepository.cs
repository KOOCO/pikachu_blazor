using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.GroupPurchaseOverviews.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupPurchaseOverviews;

public class EfCoreGroupPurchaseOverviewRepository :
    EfCoreRepository<
        PikachuDbContext,
        GroupPurchaseOverview,
        Guid
    >, IGroupPurchaseOverviewRepository
{
    #region Constructor
    public EfCoreGroupPurchaseOverviewRepository(
        IDbContextProvider<PikachuDbContext> DbContextProvider
    ) 
        : base(DbContextProvider)
    {
    }
    #endregion

    #region Methods
    public async Task<List<GroupPurchaseOverview>> GetListByGroupBuyIdAsync(Guid groupBuyId)
    {
        return [.. (await GetQueryableAsync()).Where(w => w.GroupBuyId == groupBuyId)];
    }
    #endregion
}
