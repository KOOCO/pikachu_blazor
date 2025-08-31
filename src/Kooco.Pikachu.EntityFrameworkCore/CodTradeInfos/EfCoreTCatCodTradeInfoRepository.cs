using Kooco.Pikachu.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.CodTradeInfos
{
    public class EfCoreTCatCodTradeInfoRepository : EfCoreRepository<PikachuDbContext, TCatCodTradeInfoRecord, Guid>, ITCatCodTradeInfoRepository
    {
        public EfCoreTCatCodTradeInfoRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
