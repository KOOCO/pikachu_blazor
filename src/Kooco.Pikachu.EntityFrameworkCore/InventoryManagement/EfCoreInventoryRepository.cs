using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.InventoryManagement;

public class EfCoreInventoryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : IInventoryRepository
{
    public bool? IsChangeTrackingEnabled => false;

    protected virtual Task<PikachuDbContext> GetDbContextAsync()
    {
        return dbContextProvider.GetDbContextAsync();
    }

    public async Task<long> CountAsync(
        string? filter = null, 
        Guid? itemId = null, 
        string? warehouse = null,
        string? sku = null,
        string? attributes = null,
        int? minCurrentStock = null,
        int? maxCurrentStock = null, 
        int? minAvailableStock = null, 
        int? maxAvailableStock = null
        )
    {
        var queryable = await GetFilteredQueryableAsync(
            filter, 
            itemId, 
            warehouse,
            sku,
            attributes,
            minCurrentStock, 
            maxCurrentStock,
            minAvailableStock, 
            maxAvailableStock
            );

        return await queryable.LongCountAsync();
    }

    public async Task<List<InventoryModel>> GetListAsync(
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
        )
    {
        var queryable = await GetFilteredQueryableAsync(
            filter, 
            itemId, 
            warehouse,
            sku,
            attributes,
            minCurrentStock, 
            maxCurrentStock,
            minAvailableStock, 
            maxAvailableStock
            );

        return await queryable
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(InventoryModel.ItemName) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<IQueryable<InventoryModel>> GetFilteredQueryableAsync(
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
        )
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.Items
            .AsNoTracking()
            .WhereIf(itemId.HasValue, x => x.Id == itemId)
            .SelectMany(item => item.ItemDetails, (item, detail) => new
            {
                item.ItemName,
                detail.ItemId,
                detail.Id,
                detail.SKU,
                detail.InventoryAccount,
                detail.StockOnHand,
                detail.SaleableQuantity,
                detail.PreOrderableQuantity,
                detail.SaleablePreOrderQuantity,
                detail.Attribute1Value,
                detail.Attribute2Value,
                detail.Attribute3Value
            });

        query = query
            .WhereIf(itemDetailIds is { Count: > 0}, x => itemDetailIds!.Contains(x.Id))
            .WhereIf(!string.IsNullOrWhiteSpace(warehouse), x => x.InventoryAccount == warehouse)
            .WhereIf(!string.IsNullOrWhiteSpace(sku), x => x.SKU == sku)
            .WhereIf(minCurrentStock.HasValue, x => x.StockOnHand >= minCurrentStock!.Value)
            .WhereIf(maxCurrentStock.HasValue, x => x.StockOnHand <= maxCurrentStock!.Value)
            .WhereIf(minAvailableStock.HasValue, x => x.SaleableQuantity >= minAvailableStock!.Value)
            .WhereIf(maxAvailableStock.HasValue, x => x.SaleableQuantity <= maxAvailableStock!.Value);

        var modelQuery = query.Select(x => new InventoryModel
        {
            ItemId = x.ItemId,
            ItemDetailId = x.Id,
            ItemName = x.ItemName,
            Sku = x.SKU,
            Warehouse = x.InventoryAccount,
            CurrentStock = x.StockOnHand ?? 0,
            AvailableStock = x.SaleableQuantity ?? 0,
            PreOrderQuantity = x.PreOrderableQuantity ?? 0,
            AvailablePreOrderQuantity = x.SaleablePreOrderQuantity ?? 0,
            Attributes = 
                (x.Attribute1Value != null && x.Attribute1Value != "" ? x.Attribute1Value : "") +
                (x.Attribute2Value != null && x.Attribute2Value != ""
                    ? (x.Attribute1Value != null && x.Attribute1Value != "" ? " / " : "") + x.Attribute2Value
                    : "") +
                (x.Attribute3Value != null && x.Attribute3Value != ""
                    ? ((x.Attribute1Value != null && x.Attribute1Value != "" || x.Attribute2Value != null && x.Attribute2Value != "") ? " / " : "") + x.Attribute3Value
                    : "")
        });

        if (!string.IsNullOrWhiteSpace(attributes))
        {
            modelQuery = modelQuery.Where(x => x.Attributes == attributes);
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            modelQuery = modelQuery.Where(x => x.ItemName.Contains(filter)
                || x.ItemId.ToString() == filter || x.ItemDetailId.ToString() == filter
                || x.Attributes.Contains(filter)
                || (x.Sku != null && x.Sku.Contains(filter))
                || (x.Warehouse != null && x.Warehouse.Contains(filter)));
        }

        return modelQuery;
    }

    public async Task<List<string>> GetWarehouseLookupAsync()
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.ItemDetails
            .AsNoTracking()
            .Where(id => !string.IsNullOrWhiteSpace(id.InventoryAccount))
            .Select(id => id.InventoryAccount!)
            .Distinct()
            .Order()
            .ToListAsync();
    }

    public async Task<List<string>> GetSkuLookupAsync()
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.ItemDetails
            .AsNoTracking()
            .Where(id => !string.IsNullOrWhiteSpace(id.SKU))
            .Select(id => id.SKU)
            .Distinct()
            .Order()
            .ToListAsync();
    }

    public async Task<List<string>> GetAttributesLookupAsync()
    {
        var dbContext = await GetDbContextAsync();

        var attributes = await dbContext.ItemDetails
            .AsNoTracking()
            .Select(id => new { id.Attribute1Value, id.Attribute2Value, id.Attribute3Value })
            .Distinct()
            .ToListAsync();

        return [.. attributes
            .Select(attribute =>
                string.Join(" / ", new[]
                {
                    attribute.Attribute1Value,
                    attribute.Attribute2Value,
                    attribute.Attribute3Value
                }.Where(attr => !string.IsNullOrEmpty(attr))) ?? "")
            .Distinct()
            .Order()
            ];
    }
}
