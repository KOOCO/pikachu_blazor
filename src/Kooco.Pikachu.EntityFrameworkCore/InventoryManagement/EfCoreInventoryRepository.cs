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

    public async Task<long> CountAsync(string? filter = null, Guid? itemId = null, int? minCurrentStock = null,
        int? maxCurrentStock = null, int? minAvailableStock = null, int? maxAvailableStock = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, itemId, minCurrentStock, maxCurrentStock,
            minAvailableStock, maxAvailableStock);

        return await queryable.LongCountAsync();
    }

    public async Task<List<InventoryModel>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null,
        string? filter = null, Guid? itemId = null, int? minCurrentStock = null, int? maxCurrentStock = null,
        int? minAvailableStock = null, int? maxAvailableStock = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, itemId, minCurrentStock, maxCurrentStock,
            minAvailableStock, maxAvailableStock);

        return await queryable
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(InventoryModel.ItemName) : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }

    //public async Task<IQueryable<InventoryModel>> GetFilteredQueryableAsync(string? filter = null,
    //    Guid? itemId = null, int? minCurrentStock = null, int? maxCurrentStock = null,
    //    int? minAvailableStock = null, int? maxAvailableStock = null)
    //{
    //    var dbContext = await GetDbContextAsync();

    //    var query = dbContext.Items
    //        .AsNoTracking()
    //        .WhereIf(itemId.HasValue, item => item.Id == itemId)
    //        .Include(item => item.ItemDetails)
    //        .SelectMany(item => item.ItemDetails)
    //        .Select(item => new InventoryModel
    //        {
    //            ItemId = item.ItemId,
    //            ItemDetailId = item.Id,
    //            //ItemName = item.Name,
    //            Attribute1 = item.Attribute1Value,
    //            Attribute2 = item.Attribute2Value,
    //            Attribute3 = item.Attribute3Value,
    //            Sku = item.SKU,
    //            Warehouse = item.WarehouseName,
    //            CurrentStock = item.StockOnHand.HasValue ? item.StockOnHand.Value : 0,
    //            AvailableStock = item.SaleableQuantity.HasValue ? item.SaleableQuantity.Value : 0,
    //            PreOrderQuantity = item.PreOrderableQuantity.HasValue ? item.PreOrderableQuantity.Value : 0,
    //            AvailablePreOrderQuantity = item.SaleablePreOrderQuantity.HasValue ? item.SaleablePreOrderQuantity.Value : 0,
    //        });

    //    return query
    //        .WhereIf(minCurrentStock.HasValue, item => item.CurrentStock >= minCurrentStock)
    //        .WhereIf(maxCurrentStock.HasValue, item => item.CurrentStock <= maxCurrentStock)
    //        .WhereIf(minAvailableStock.HasValue, item => item.AvailableStock >= minAvailableStock)
    //        .WhereIf(maxAvailableStock.HasValue, item => item.AvailableStock <= maxAvailableStock)
    //        .WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.ItemName.Contains(filter)
    //            || item.ItemId.ToString() == filter || item.ItemDetailId.ToString() == filter
    //            || item.Attributes.Contains(filter)
    //            || (item.Sku != null && item.Sku.Contains(filter))
    //            || (item.Warehouse != null && item.Warehouse.Contains(filter)));
    //}

    public async Task<IQueryable<InventoryModel>> GetFilteredQueryableAsync(
        string? filter = null,
        Guid? itemId = null,
        int? minCurrentStock = null,
        int? maxCurrentStock = null,
        int? minAvailableStock = null,
        int? maxAvailableStock = null
        )
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.Items
            .WhereIf(itemId.HasValue, x => x.Id == itemId)
            .SelectMany(item => item.ItemDetails, (item, detail) => new
            {
                item.ItemName,
                detail.ItemId,
                detail.Id,
                detail.SKU,
                detail.WarehouseName,
                detail.StockOnHand,
                detail.SaleableQuantity,
                detail.PreOrderableQuantity,
                detail.SaleablePreOrderQuantity,
                detail.Attribute1Value,
                detail.Attribute2Value,
                detail.Attribute3Value
            });

        query = query
            .WhereIf(minCurrentStock.HasValue, x => x.StockOnHand >= minCurrentStock!.Value)
            .WhereIf(maxCurrentStock.HasValue, x => x.StockOnHand <= maxCurrentStock!.Value)
            .WhereIf(minAvailableStock.HasValue, x => x.SaleableQuantity >= minAvailableStock!.Value)
            .WhereIf(maxAvailableStock.HasValue, x => x.SaleableQuantity <= maxAvailableStock!.Value);

        var modelQuery = query.Select(x => new InventoryModel
        {
            ItemId = x.ItemId,
            ItemDetailId = x.Id,
            ItemName = x.ItemName,
            Attribute1 = x.Attribute1Value,
            Attribute2 = x.Attribute2Value,
            Attribute3 = x.Attribute3Value,
            Sku = x.SKU,
            Warehouse = x.WarehouseName,
            CurrentStock = x.StockOnHand ?? 0,
            AvailableStock = x.SaleableQuantity ?? 0,
            PreOrderQuantity = x.PreOrderableQuantity ?? 0,
            AvailablePreOrderQuantity = x.SaleablePreOrderQuantity ?? 0,
        });

        if (!string.IsNullOrWhiteSpace(filter))
        {
            modelQuery = modelQuery.Where(item => item.ItemName.Contains(filter)
                || item.ItemId.ToString() == filter || item.ItemDetailId.ToString() == filter
                || item.Attributes.Contains(filter)
                || (item.Sku != null && item.Sku.Contains(filter))
                || (item.Warehouse != null && item.Warehouse.Contains(filter)));
        }

        return modelQuery;
    }
}
