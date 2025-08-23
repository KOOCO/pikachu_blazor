using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.CodTradeInfos;

public class EfCoreEcPayCodTradeInfoRepository : EfCoreRepository<PikachuDbContext, EcPayCodTradeInfoRecord, Guid>, IEcPayCodTradeInfoRepository
{
    public EfCoreEcPayCodTradeInfoRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<(Guid OrderId, string MerchantTradeNo)>> GetMerchantTradeNos(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query = await (
            from o in dbContext.Orders.AsNoTracking()
            where o.PaymentMethod == PaymentMethods.CashOnDelivery
                && o.CreationTime.Date >= cutoffDate
            join t in dbContext.OrderTradeNos.AsNoTracking()
                on o.Id equals t.OrderId
            where !dbContext.EcPayCodTradeInfoRecords
                            .Any(r => r.MerchantTradeNo == t.MarchentTradeNo)
            select new { o.Id, t.MarchentTradeNo }
        ).ToListAsync(cancellationToken);

        return [.. query.Select(q => (q.Id, q.MarchentTradeNo))];
    }
}
