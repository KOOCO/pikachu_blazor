using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.InventoryManagement;

public interface IInventoryRepository : IRepository
{
    Task<long> CountAsync(
        string? filter = null, 
        Guid? itemId = null,
        string? warehouse = null,
        string? sku = null,
        string? attributes = null,
        int? minCurrentStock = null,
        int? maxCurrentStock = null, 
        int? minAvailableStock = null, 
        int? maxAvailableStock = null
        );

    Task<List<InventoryModel>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = 10, 
        string? sorting = null,
        string? filter = null, 
        Guid? itemId = null,
        string? warehouse = null,
        string? sku = null,
        string? attributes = null,
        int? minCurrentStock = null, 
        int? maxCurrentStock = null,
        int? minAvailableStock = null, 
        int? maxAvailableStock = null
        );

    Task<IQueryable<InventoryModel>> GetFilteredQueryableAsync(
        string? filter = null,
        Guid? itemId = null,
        string? warehouse = null,
        string? sku = null,
        string? attributes = null,
        int? minCurrentStock = null,
        int? maxCurrentStock = null,
        int? minAvailableStock = null,
        int? maxAvailableStock = null,
        List<Guid>? itemDetailIds = null
        );

    Task<List<string>> GetWarehouseLookupAsync();
    Task<List<string>> GetSkuLookupAsync();
    Task<List<string>> GetAttributesLookupAsync();
}
