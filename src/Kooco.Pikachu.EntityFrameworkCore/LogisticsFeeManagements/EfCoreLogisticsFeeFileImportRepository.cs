using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
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
    public class EfCoreLogisticsFeeFileImportRepository : EfCoreRepository<PikachuDbContext, LogisticsFeeFileImport, Guid>, ILogisticsFeeFileImportRepository
    {
        public EfCoreLogisticsFeeFileImportRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<LogisticsFeeFileImport>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string filter = null,
            LogisticsFileType? fileType = null,
            FileProcessingStatus? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            return await queryable
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.OriginalFileName.Contains(filter) || x.ProcessingNotes.Contains(filter))
                .WhereIf(fileType.HasValue, x => x.FileType == fileType.Value)
                .WhereIf(status.HasValue, x => x.BatchStatus == status.Value)
                .WhereIf(startDate.HasValue, x => x.UploadDate >= startDate.Value)
                .WhereIf(endDate.HasValue, x => x.UploadDate <= endDate.Value)
                .Where(x=>x.TotalRecords>0)
                .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(LogisticsFeeFileImport.UploadDate) + " desc" : sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filter = null,
            LogisticsFileType? fileType = null,
            FileProcessingStatus? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            return await queryable
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.OriginalFileName.Contains(filter) || x.ProcessingNotes.Contains(filter))
                .WhereIf(fileType.HasValue, x => x.FileType == fileType.Value)
                .WhereIf(status.HasValue, x => x.BatchStatus == status.Value)
                .WhereIf(startDate.HasValue, x => x.UploadDate >= startDate.Value)
                .WhereIf(endDate.HasValue, x => x.UploadDate <= endDate.Value)
                .Where(x => x.TotalRecords > 0)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<LogisticsFeeFileImport> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();

            return await queryable
                .Include(x => x.LogisticsFeeTenantSummaries)
                .Include(x => x.TenantLogisticsFeeRecords)
                .FirstOrDefaultAsync(x => x.Id == id, GetCancellationToken(cancellationToken));
        }
    }

}
