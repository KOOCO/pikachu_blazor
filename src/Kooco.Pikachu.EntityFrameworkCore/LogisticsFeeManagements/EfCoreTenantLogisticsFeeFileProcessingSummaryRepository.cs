using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class EfCoreTenantLogisticsFeeFileProcessingSummaryRepository : EfCoreRepository<PikachuDbContext, TenantLogisticsFeeFileProcessingSummary, Guid>, ITenantLogisticsFeeFileProcessingSummaryRepository
    {
        public EfCoreTenantLogisticsFeeFileProcessingSummaryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<TenantLogisticsFeeFileProcessingSummary>> GetByFileImportIdAsync(
            Guid fileImportId,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();
            return await queryable
                .Where(x => x.FileImportId == fileImportId)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<TenantLogisticsFeeFileProcessingSummary>> GetByTenantIdAsync(
            Guid? tenantId,
            int skipCount,
            int maxResultCount,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var queryable = await GetQueryableAsync();

            var tenantQueryable = dbContext.Tenants; // Injected tenant repository
            var walletQueryable = dbContext.TenantWallets; // Injected wallet repository

            // 1) Find latest ProcessedAt per tenant (optionally filtered by tenantId)
            var latestPerTenant =
                from s in queryable
                where !tenantId.HasValue || s.TenantId == tenantId
                group s by s.TenantId into g
                select new
                {
                    TenantId = g.Key,
                    MaxProcessedAt = g.Max(x => x.ProcessedAt)
                };
           var  latestPerTenant1 =
                (from s in queryable
                 where !tenantId.HasValue || s.TenantId == tenantId
                 group s by s.TenantId into g
                 select new
                 {
                     TenantId = g.Key,
                     MaxProcessedAt = g.Max(x => x.ProcessedAt)
                 }).ToList();

            // 2) Join back to get the full row, then left-join wallet and tenant
            var query =
                from lp in latestPerTenant
                join s in queryable
                    on new { lp.TenantId, lp.MaxProcessedAt }
                    equals new { TenantId = s.TenantId, MaxProcessedAt = s.ProcessedAt }
                join w in walletQueryable on s.TenantId equals w.TenantId into wj
                from w in wj.DefaultIfEmpty()
                join t in tenantQueryable on s.TenantId equals (Guid?)t.Id into tj
                from t in tj.DefaultIfEmpty()
                orderby s.ProcessedAt descending
                select new TenantLogisticsFeeFileProcessingSummary
                {
                    TenantTotalRecords = s.TenantTotalRecords,
                    TenantTotalAmount = s.TenantTotalAmount,
                    TenantSuccessfulRecords = s.TenantSuccessfulRecords,
                    TenantFailedRecords = s.TenantFailedRecords,
                    ProcessedAt = s.ProcessedAt,
                    TenantBatchStatus = s.TenantBatchStatus,
                    TenantId = t != null ? t.Id : s.TenantId ?? Guid.Empty,
                    WalletId = w != null ? w.Id : Guid.Empty,
                    WalletBalance = w != null ? w.WalletBalance : 0,
                    TenantName = t != null ? t.Name : ""
                };

            return await query
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }
}
