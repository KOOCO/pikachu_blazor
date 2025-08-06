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
            Guid tenantId,
            int skipCount,
            int maxResultCount,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();
            return await queryable
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.ProcessedAt)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }
}
