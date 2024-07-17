using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Groupbuys.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupBuyItemGroupsDetails;

public class EfCoreGroupBuyItemGroupDetailsRepository :
    EfCoreRepository<
        PikachuDbContext,
        GroupBuyItemGroupDetails,
        Guid
    >, IGroupBuyItemGroupDetailsRepository
{
    #region Constructor
    public EfCoreGroupBuyItemGroupDetailsRepository(
        IDbContextProvider<PikachuDbContext> DbContextProvider
    ) : base(DbContextProvider)
    {
    }
    #endregion
}
