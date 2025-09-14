using Kooco.Pikachu.Common;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantPaymentFees;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
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
                TotalFees = g.payout.Sum(p => p.HandlingFee + p.ProcessingFee),
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
                TotalFees = g.Sum(r => r.HandlingFee + r.ProcessingFee),
                Transactions = g.Count(),
                AvgFeeRate = g.Average(r => r.FeeRate)
            })
            .Distinct()
            .ToListAsync();
    }

    public async Task<TenantPayoutDetailSummary> GetTenantPayoutDetailSummaryAsync(
        Guid tenantId, 
        PaymentFeeType feeType, 
        DateTime startDate,
        DateTime endDate,
        PaymentMethods? paymentMethod = null,
        string? filter = null,
        CancellationToken cancellationToken = default
        )
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.TenantPayoutRecords
            .Where(r => r.TenantId == tenantId && r.FeeType == feeType)
            .WhereIf(paymentMethod.HasValue, x => x.PaymentMethod == paymentMethod)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.OrderNo == filter || x.Id.ToString() == filter)
            .GroupBy(r => r.CreationTime.Date)
            .Where(g => g.Key >= startDate.Date && g.Key <= endDate.Date)
            .Select(g => new TenantPayoutDetailSummary
            {
                NetAmount = g.Sum(r => r.NetAmount),
                TotalFees = g.Sum(r => r.HandlingFee + r.ProcessingFee),
                TotalAmount = g.Sum(r => r.GrossOrderAmount),
                Transactions = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResultModel<TenantPayoutRecord>> GetListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        Guid tenantId,
        PaymentFeeType feeType,
        DateTime startDate,
        DateTime endDate,
        PaymentMethods? paymentMethod = null,
        string? filter = null,
        CancellationToken cancellationToken = default
        )
    {
        var dbContext = await GetDbContextAsync();

        filter = filter?.Trim();
        sorting = string.IsNullOrWhiteSpace(sorting) ? $"{nameof(TenantPayoutRecord.OrderCreationTime)} DESC" : sorting;

        var projection = dbContext.TenantPayoutRecords
            .Where(x => x.TenantId == tenantId && x.FeeType == feeType)
            .Where(o => o.OrderCreationTime.Date >= startDate.Date && o.OrderCreationTime.Date <= endDate.Date)
            .WhereIf(paymentMethod.HasValue, x => x.PaymentMethod == paymentMethod)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.OrderNo == filter || x.Id.ToString() == filter);

        var totalCount = await projection.LongCountAsync(cancellationToken);

        var items = await projection
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);

        return new PagedResultModel<TenantPayoutRecord>
        {
            TotalCount = totalCount,
            Items = items
        };
    }
}
