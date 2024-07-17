using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Groupbuys.Interface;
using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupBuyItemGroups;

public class EfCoreGroupBuyItemGroupsRepository :
    EfCoreRepository<
        PikachuDbContext, 
        GroupBuyItemGroup, 
        Guid
    >, IGroupBuyItemGroupsRepository
{
    #region Constructor
    public EfCoreGroupBuyItemGroupsRepository(
        IDbContextProvider<PikachuDbContext> DbContextProvider
    ) : base(DbContextProvider)
    {
    }
    #endregion
}
