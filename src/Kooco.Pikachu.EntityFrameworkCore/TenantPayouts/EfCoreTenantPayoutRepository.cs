using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.TenantPaymentFees;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<PaymentFeeType>> GetActivePaymentProvidersAsync(Guid tenantId)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.TenantPaymentFees
            .Where(f => f.TenantId == tenantId && f.IsEnabled)
            .GroupBy(f => f.FeeType)
            .Select(g => g.Key)
            .ToListAsync();
    }

    public async Task<List<TenantPayoutYearlySummary>> GetTenantPayoutYearlySummariesAsync(Guid tenantId, PaymentFeeType feeType)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.TenantPayoutRecords
            .Where(r => r.TenantId == tenantId && r.FeeType == feeType)
            .GroupBy(r => r.CreationTime.Year)
            .Select(g => new TenantPayoutYearlySummary
            {
                Year = g.Key,
                TotalFees = g.Sum(r => r.NetAmount),
                Transactions = g.Count(),
                AvgFeeRate = g.Average(r => r.FeeRate)
            })
            .Distinct()
            .ToListAsync();
    }
}
