using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Groupbuys.Interface;
using Kooco.Pikachu.Groupbuys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupBuyItemsPriceses
{
    public class EFCoreGroupBuyItemsPriceRepository :
    EfCoreRepository<
        PikachuDbContext,
        GroupBuyItemsPrice,
        Guid
    >, IGroupBuyItemsPriceRepository
    {
        #region Constructor
        public EFCoreGroupBuyItemsPriceRepository(
            IDbContextProvider<PikachuDbContext> DbContextProvider
        ) : base(DbContextProvider)
        {
        }
        #endregion
    }
}
