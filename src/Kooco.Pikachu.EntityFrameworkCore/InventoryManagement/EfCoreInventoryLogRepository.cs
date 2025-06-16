using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.InventoryManagement;

public class EfCoreInventoryLogRepository : EfCoreRepository<PikachuDbContext, InventoryLog, Guid>, IInventoryLogRepository
{
    public EfCoreInventoryLogRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> CountAsync(Guid itemId, Guid itemDetailId)
    {
        var queryable = await GetFilteredQueryableAsync(itemId, itemDetailId);
        return await queryable.LongCountAsync();
    }

    public async Task<List<InventoryLog>> GetListAsync(
        Guid itemId,
        Guid itemDetailId,
        int skipCount = 0,
        int maxResultCount = 30,
        string? sorting = null
        )
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            sorting = nameof(InventoryLog.CreationTime) + " DESC";
        }

        var queryable = await GetFilteredQueryableAsync(itemId, itemDetailId);

        return await queryable
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<IQueryable<InventoryLog>> GetFilteredQueryableAsync(Guid itemId, Guid itemDetailId)
    {
        var queryable = (await GetQueryableAsync()).AsNoTracking();

        return queryable
            .Where(q => q.ItemId == itemId && q.ItemDetailId == itemDetailId);
    }
}