using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryLogRepository : IRepository<InventoryLog, Guid>
{
    Task<List<InventoryLog>> GetListAsync(Guid itemId, Guid itemDetailId);
}
