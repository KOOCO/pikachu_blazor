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

            var tenantQueryable =  dbContext.Tenants; // Injected tenant repository
            var walletQueryable = dbContext.TenantWallets; // Injected wallet repository

            var query =
                from summary in queryable
                join wallet in walletQueryable on summary.TenantId equals wallet.TenantId into walletJoin
                from wallet in walletJoin.DefaultIfEmpty()
                join tenant in tenantQueryable on summary.TenantId equals tenant.Id into tenantJoin
                from tenant in tenantJoin.DefaultIfEmpty()
                where !tenantId.HasValue || summary.TenantId == tenantId
                orderby summary.ProcessedAt descending
                select new TenantLogisticsFeeFileProcessingSummary
                {
                    TenantTotalRecords = summary.TenantTotalRecords,
                    TenantTotalAmount = summary.TenantTotalAmount,
                    TenantSuccessfulRecords = summary.TenantSuccessfulRecords,
                    TenantFailedRecords = summary.TenantFailedRecords,
                    ProcessedAt = summary.ProcessedAt,
                    TenantBatchStatus = summary.TenantBatchStatus,
                    TenantId=tenant.Id,
                  
                    WalletId = wallet != null ? wallet.Id : Guid.Empty,
                    WalletBalance = wallet != null ? wallet.WalletBalance : 0,
                    TenantName = tenant != null ? tenant.Name : ""
                };

            return await query
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }
}
