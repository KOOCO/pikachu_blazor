using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class EfCoreTenantDeliveryFeeRepository : EfCoreRepository<PikachuDbContext, TenantDeliveryFee, Guid>,
          ITenantDeliveryFeeRepository
    {
        public EfCoreTenantDeliveryFeeRepository(
            IDbContextProvider<PikachuDbContext> dbContextProvider
        ) : base(dbContextProvider)
        {
        }

        // ---- Helper to get base query (you can toggle AsNoTracking for read paths) ----
        private async Task<IQueryable<TenantDeliveryFee>> GetBaseQueryAsync(bool asNoTracking = true)
        {
            var dbSet = await GetDbSetAsync();
            return asNoTracking ? dbSet.AsNoTracking() : dbSet.AsQueryable();
        }

        // ---- Centralized filter composition reused by both list & count ----
        private IQueryable<TenantDeliveryFee> ApplyFilter(
            IQueryable<TenantDeliveryFee> query,
            Guid? tenantId,
            DeliveryProvider? deliveryProvider,
            bool? isEnabled,
            FeeKind? feeKind)
        {
            return query
                .WhereIf(tenantId.HasValue, x => x.TenantId == tenantId)
                .WhereIf(deliveryProvider.HasValue, x => x.DeliveryProvider == deliveryProvider)
                .WhereIf(isEnabled.HasValue, x => x.IsEnabled == isEnabled.Value)
                .WhereIf(feeKind.HasValue, x => x.FeeKind == feeKind.Value);
        }

        public async Task<List<TenantDeliveryFee>> GetByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            var query = await GetBaseQueryAsync();
            query = query.Where(x => x.TenantId == tenantId);
            return await AsyncExecuter.ToListAsync(query, cancellationToken);
        }
        public async Task<TenantDeliveryFee> GetByTenantIdAndDeliveryTypeAsync(
           Guid tenantId,
           DeliveryProvider deliveryProvider,
           CancellationToken cancellationToken = default)
        {
            var query = await GetBaseQueryAsync();
            query = query.Where(x => x.TenantId == tenantId && x.DeliveryProvider == deliveryProvider);
            return await AsyncExecuter.FirstOrDefaultAsync(query, cancellationToken);
        }
        public async Task<List<TenantDeliveryFee>> GetListAsync(
            Guid? tenantId = null,
            DeliveryProvider? deliveryProvider = null,
            bool? isEnabled = null,
            FeeKind? feeKind = null,
            string? sorting = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            CancellationToken cancellationToken = default)
        {
            var query = await GetBaseQueryAsync();

            query = ApplyFilter(query, tenantId, deliveryProvider, isEnabled, feeKind);

            // Default sorting if none is provided
            sorting = string.IsNullOrWhiteSpace(sorting)
                ? nameof(TenantDeliveryFee.CreationTime) + " DESC"
                : sorting;

            query = query.OrderBy(sorting)
                         .Skip(skipCount)
                         .Take(maxResultCount);

            return await AsyncExecuter.ToListAsync(query, cancellationToken);
        }

        public async Task<long> GetCountAsync(
            Guid? tenantId = null,
            DeliveryProvider? deliveryProvider = null,
            bool? isEnabled = null,
            FeeKind? feeKind = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetBaseQueryAsync();
            query = ApplyFilter(query, tenantId, deliveryProvider, isEnabled, feeKind);
            return await AsyncExecuter.LongCountAsync(query, cancellationToken);
        }
        public async Task<(List<TenantLogisticsFeeOverviewItem> Items, long TotalCount)> GetTenantLogisticsFeeOverviewAsync(
           string? tenantNameFilter = null,
           string? sorting = null,
           int skipCount = 0,
           int maxResultCount = int.MaxValue,
           CancellationToken cancellationToken = default
            )
        {
            // Default sorting by Tenant.Name
            sorting ??= nameof(Tenant.Name) + " ASC";

            // Cross-tenant read in case TenantDeliveryFee implements IMultiTenant
            using (DataFilter.Disable<IMultiTenant>())
            {
                var ctx = await GetDbContextAsync();

                // ---- Base tenant query (left side) ----
                IQueryable<Tenant> tenantQ = ctx.Set<Tenant>().AsNoTracking();

                if (!string.IsNullOrWhiteSpace(tenantNameFilter))
                {
                    tenantQ = tenantQ.Where(t => t.Id.ToString() == tenantNameFilter || t.Name.Contains(tenantNameFilter));
                }

                var totalCount = await tenantQ.LongCountAsync(cancellationToken);

                var pageTenants = await tenantQ
                    .OrderBy(sorting)                      // uses System.Linq.Dynamic.Core
                    .Skip(skipCount)
                    .Take(maxResultCount)
                    .ToListAsync(cancellationToken);

                if (pageTenants.Count == 0)
                    return (new List<TenantLogisticsFeeOverviewItem>(), totalCount);

                var pageTenantIds = pageTenants.Select(t => t.Id).ToList();

                // ---- Aggregate fees for tenants on this page ----
                var feeQ = (await GetDbSetAsync()).AsNoTracking();

                var feeAgg = await feeQ
                    .Where(f => f.TenantId != null && pageTenantIds.Contains(f.TenantId.Value))
                    .GroupBy(f => f.TenantId!.Value)
                    .Select(g => new
                    {
                        TenantId = g.Key,
                        AnyEnabled = g.Any(x => x.IsEnabled),
                        LastModified = g.Max(x => (DateTime?)(x.LastModificationTime ?? x.CreationTime))
                    })
                    .ToListAsync(cancellationToken);

                var feeDict = feeAgg.ToDictionary(x => x.TenantId, x => x);

                var paymentFeeAgg = await ctx.TenantPaymentFees.AsNoTracking()
                    .Where(tpf => pageTenantIds.Contains(tpf.TenantId))
                    .GroupBy(tpf => tpf.TenantId)
                    .Select(g => new
                    {
                        TenantId = g.Key,
                        AnyEnabled = g.Any(x => x.IsEnabled),
                        LastModified = g.Max(x => (DateTime?)(x.LastModificationTime ?? x.CreationTime))
                    })
                    .ToListAsync(cancellationToken);

                var paymentFeeDict = paymentFeeAgg.ToDictionary(pf => pf.TenantId);

                // ---- Left-join shape ----
                var items = pageTenants.Select(t =>
                {
                    var has = feeDict.TryGetValue(t.Id, out var agg);
                    var hasPaymentFee = paymentFeeDict.TryGetValue(t.Id, out var paymentFeeAgg);
                    return new TenantLogisticsFeeOverviewItem
                    {
                        TenantId = t.Id,
                        TenantName = t.Name,
                        PaymentFeeStatus = hasPaymentFee && paymentFeeAgg!.AnyEnabled,
                        LogisticsFeeStatus = has && agg!.AnyEnabled,
                        LastModificationTime = new[] { agg?.LastModified, paymentFeeAgg?.LastModified }.Max()
                    };
                }).ToList();

                return (items, totalCount);
            }
        }
    }
}
