using Kooco.Pikachu.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.CodTradeInfos;

public class EfCoreEcPayCodTradeInfoRepository : EfCoreRepository<PikachuDbContext, EcPayCodTradeInfoRecord, Guid>, IEcPayCodTradeInfoRepository
{
    public EfCoreEcPayCodTradeInfoRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
