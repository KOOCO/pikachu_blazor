using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryRepository : IRepository
{
    Task<long> CountAsync(string? filter = null, Guid? itemId = null, int? minCurrentStock = null,
        int? maxCurrentStock = null, int? minAvailableStock = null, int? maxAvailableStock = null);

    Task<List<InventoryModel>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null,
        string? filter = null, Guid? itemId = null, int? minCurrentStock = null, int? maxCurrentStock = null,
        int? minAvailableStock = null, int? maxAvailableStock = null);
}
