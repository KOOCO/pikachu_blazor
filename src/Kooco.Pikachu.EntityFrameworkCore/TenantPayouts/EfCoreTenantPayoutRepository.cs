using JetBrains.Annotations;
using Kooco.Pikachu.Common;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantPaymentFees;
using Microsoft.EntityFrameworkCore;
using Polly;
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

    public async Task<PagedResultModel<TenantPayoutSummary>> GetTenantSummariesAsync(
        int skipCount,
        int maxResultCount,
        string? sorting,
        string? filter,
        CancellationToken cancellationToken = default
        )
    {
        var dbContext = await GetDbContextAsync();
        filter = filter?.Trim();

        var summaries = dbContext.Tenants
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
            .OrderBy(s => s.Name)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.TenantId.ToString() == filter || x.Name.Contains(filter));

        return new PagedResultModel<TenantPayoutSummary>
        {
            TotalCount = await summaries.LongCountAsync(cancellationToken),
            Items = await summaries
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(cancellationToken)
        };
    }

    public async Task<List<PaymentFeeType>> GetActivePaymentProvidersAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var feeTypes = await dbContext.TenantPayoutRecords
            .Where(r => r.TenantId == tenantId)
            .Select(r => r.FeeType)
            .Distinct()
            .ToListAsync(cancellationToken);
        
        return feeTypes;
    }

    public async Task<List<TenantPayoutYearlySummary>> GetTenantPayoutYearlySummariesAsync(Guid tenantId, PaymentFeeType feeType, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.TenantPayoutRecords
            .Where(r => r.TenantId == tenantId && r.FeeType == feeType)
            .GroupBy(r => r.OrderCreationTime.Year)
            .Select(g => new TenantPayoutYearlySummary
            {
                Year = g.Key,
                TotalFees = g.Sum(r => r.HandlingFee + r.ProcessingFee),
                Transactions = g.Count(),
                AvgFeeRate = g.Average(r => r.FeeRate)
            })
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<TenantPayoutDetailSummary> GetTenantPayoutDetailSummaryAsync(
        Guid tenantId,
        PaymentFeeType feeType,
        DateTime startDate,
        DateTime endDate,
        PaymentMethods? paymentMethod = null,
        string? filter = null,
        bool? isPaid = null,
        CancellationToken cancellationToken = default
        )
    {
        var q = await GetFilteredQueryableAsync(
            tenantId,
            feeType,
            startDate,
            endDate,
            paymentMethod,
            filter,
            isPaid
            );

        return new TenantPayoutDetailSummary
        {
            NetAmount = await q.SumAsync(r => r.NetAmount, cancellationToken),
            TotalFees = await q.SumAsync(r => r.HandlingFee + r.ProcessingFee, cancellationToken),
            TotalAmount = await q.SumAsync(r => r.GrossOrderAmount, cancellationToken),
            Transactions = await q.CountAsync(cancellationToken)
        };
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
        bool? isPaid = null,
        CancellationToken cancellationToken = default
        )
    {
        var q = await GetFilteredQueryableAsync(
            tenantId,
            feeType,
            startDate,
            endDate,
            paymentMethod,
            filter,
            isPaid
            );

        var totalCount = await q.LongCountAsync(cancellationToken);

        var items = await q
            .OrderBy(!string.IsNullOrWhiteSpace(sorting) ? sorting : "CreationTime DESC")
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);

        return new PagedResultModel<TenantPayoutRecord>
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<IQueryable<TenantPayoutRecord>> GetFilteredQueryableAsync(
        Guid tenantId,
        PaymentFeeType feeType,
        DateTime startDate,
        DateTime endDate,
        PaymentMethods? paymentMethod = null,
        string? filter = null,
        bool? isPaid = null
        )
    {
        var q = await GetQueryableAsync();

        filter = filter?.Trim();

        q = q
            .Where(x => x.TenantId == tenantId && x.FeeType == feeType)
            .Where(o => o.OrderCreationTime.Date >= startDate.Date && o.OrderCreationTime.Date <= endDate.Date)
            .WhereIf(isPaid.HasValue, x => x.IsPaid == isPaid)
            .WhereIf(paymentMethod.HasValue, x => x.PaymentMethod == paymentMethod)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.OrderNo == filter || x.Id.ToString() == filter);

        return q;
    }

    public async Task MarkAsPaidAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var q = await GetQueryableAsync();

        await q
            .Where(r => ids.Contains(r.Id) && !r.IsPaid)
            .ExecuteUpdateAsync(r =>
                r.SetProperty(p => p.IsPaid, true)
                .SetProperty(p => p.PaidTime, DateTime.Now),
                cancellationToken
                );
    }
}
