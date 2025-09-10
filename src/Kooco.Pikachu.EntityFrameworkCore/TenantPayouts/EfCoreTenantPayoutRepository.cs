using Kooco.Pikachu.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.TenantPayouts;

public class EfCoreTenantPayoutRepository : EfCoreRepository<PikachuDbContext, TenantPayoutRecord, Guid>, ITenantPayoutRepository
{
    public EfCoreTenantPayoutRepository(
        IDbContextProvider<PikachuDbContext> dbContextProvider
        ) : base(dbContextProvider)
    {
    }

    public async Task<List<TenantPayoutSummary>> GetTenantSummariesAsync()
    {
        var dbContext = await GetDbContextAsync();

        return [.. dbContext.Tenants
            .GroupJoin(
                dbContext.TenantPayoutRecords,
                tenant => tenant.Id,
                payout => payout.TenantId,
                (tenant, payout) => new { tenant, payout }
            ).Select(g => new TenantPayoutSummary
            {
                TenantId = g.tenant.Id,
                Name = g.tenant.Name,
                CreationTime = g.tenant.CreationTime,
                TotalFees = g.payout.Sum(p => p.NetAmount),
                TotalTransactions = g.payout.Count()
            })
            .OrderBy(s => s.Name)];
    }
}
