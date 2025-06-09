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

    public async Task<List<InventoryLog>> GetListAsync(Guid itemId, Guid itemDetailId)
    {
        var queryable = (await GetQueryableAsync()).AsNoTracking();

        return await queryable
            .Where(q => q.ItemId == itemId && q.ItemDetailId == itemDetailId)
            .OrderByDescending(q => q.CreationTime)
            .ToListAsync();
    }
}