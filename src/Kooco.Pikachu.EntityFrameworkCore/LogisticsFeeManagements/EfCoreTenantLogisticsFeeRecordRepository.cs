using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class EfCoreTenantLogisticsFeeRecordRepository : EfCoreRepository<PikachuDbContext, TenantLogisticsFeeRecord, Guid>, ITenantLogisticsFeeRecordRepository
    {
        public EfCoreTenantLogisticsFeeRecordRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<TenantLogisticsFeeRecord>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string? filter = null,
            Guid? tenantId = null,
            Guid? fileImportId = null,
            LogisticsFileType? fileType = null,
            WalletDeductionStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            return await queryable
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.OrderNumber.Contains(filter) || x.FailureReason.Contains(filter))
                .WhereIf(tenantId.HasValue, x => x.TenantId == tenantId.Value)
                .WhereIf(fileImportId.HasValue, x => x.FileImportId == fileImportId.Value)
                .WhereIf(fileType.HasValue, x => x.FileType == fileType.Value)
                .WhereIf(status.HasValue, x => x.DeductionStatus == status.Value)
                .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(TenantLogisticsFeeRecord.ProcessedAt) + " desc" : sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string? filter = null,
            Guid? tenantId = null,
            Guid? fileImportId = null,
            LogisticsFileType? fileType = null,
            WalletDeductionStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            return await queryable
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.OrderNumber.Contains(filter) || x.FailureReason.Contains(filter))
                .WhereIf(tenantId.HasValue, x => x.TenantId == tenantId.Value)
                .WhereIf(fileImportId.HasValue, x => x.FileImportId == fileImportId.Value)
                .WhereIf(fileType.HasValue, x => x.FileType == fileType.Value)
                .WhereIf(status.HasValue, x => x.DeductionStatus == status.Value)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<TenantLogisticsFeeRecord>> GetByIdsAsync(
            List<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();
            return await queryable
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<TenantLogisticsFeeRecord>> GetFailedRecordsAsync(
            Guid? fileImportId = null,
            Guid? tenantId = null,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            return await queryable
                .Where(x => x.DeductionStatus == WalletDeductionStatus.Failed)
                .WhereIf(fileImportId.HasValue, x => x.FileImportId == fileImportId.Value)
                .WhereIf(tenantId.HasValue, x => x.TenantId == tenantId.Value)
                .OrderByDescending(x => x.ProcessedAt)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }

}
