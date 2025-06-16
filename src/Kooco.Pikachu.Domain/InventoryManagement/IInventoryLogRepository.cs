using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryLogRepository : IRepository<InventoryLog, Guid>
{
    Task<long> CountAsync(Guid itemId, Guid itemDetailId);
    Task<List<InventoryLog>> GetListAsync(
        Guid itemId,
        Guid itemDetailId,
        int skipCount = 0,
        int maxResultCount = 30,
        string? sorting = null
        );

    Task<IQueryable<InventoryLog>> GetFilteredQueryableAsync(Guid itemId, Guid itemDetailId);
}
