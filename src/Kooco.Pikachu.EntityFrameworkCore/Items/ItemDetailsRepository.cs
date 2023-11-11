using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Orders;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Kooco.Pikachu.Items;

public class ItemDetailsRepository : EfCoreRepository<PikachuDbContext, ItemDetails, Guid>, IItemDetailsRepository
{
    public ItemDetailsRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<ItemDetails>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
    public async Task<long> CountAsync(string? filter)
    {
        return await ApplyFilters((await GetQueryableAsync()), filter).CountAsync();
    }
    public async Task<List<ItemDetails>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter)
    {
       

        return await ApplyFilters(await GetQueryableAsync(), filter)
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            
            .ToListAsync();
    }

    public async Task<List<ItemDetails>> GetInventroyListAsync(int skipCount, int maxResultCount, string? sorting, string? filter)
    {
        var dbContext = await GetDbContextAsync();
        var query = from item in dbContext.Items
                    join detailItem in dbContext.ItemDetails on item.Id equals detailItem.ItemId
                    select new ItemDetails
                    {
                        TenantId = item.TenantId,
                        ItemId = item.Id,
                        ItemName = item.ItemName,
                        SKU = detailItem.SKU,
                        SellingPrice = detailItem.SellingPrice,
                        GroupBuyPrice = detailItem.GroupBuyPrice,
                        SaleableQuantity = detailItem.SaleableQuantity,
                        PreOrderableQuantity = detailItem.PreOrderableQuantity,
                        SaleablePreOrderQuantity = detailItem.SaleablePreOrderQuantity,
                        LimitQuantity = detailItem.LimitQuantity,
                        ReorderLevel = detailItem.ReorderLevel,
                        WarehouseName = detailItem.WarehouseName,
                        InventoryAccount = detailItem.InventoryAccount,
                        OpeningStock = detailItem.OpeningStock,
                        OpeningStockValue = detailItem.OpeningStockValue,
                        PurchasePrice = detailItem.PurchasePrice,
                        PurchaseAccount = detailItem.PurchaseAccount,
                        PurchaseDescription = detailItem.PurchaseDescription,
                        PreferredVendor = detailItem.PreferredVendor,
                        StockOnHand = detailItem.StockOnHand,
                        PartNumber = detailItem.PartNumber,
                        ItemType = detailItem.ItemType,
                        ItemDetailTitle = detailItem.ItemDetailTitle,
                        ItemDetailStatus = detailItem.ItemDetailStatus,
                        ItemDetailDescription = detailItem.ItemDetailDescription,
                        Property1 = detailItem.Property1,
                        Property2 = detailItem.Property2,
                        Property3 = detailItem.Property3,
                        Attribute1Value = detailItem.Attribute1Value,
                        Attribute2Value = detailItem.Attribute2Value,
                        Attribute3Value = detailItem.Attribute3Value
                    };

        // Execute the query and materialize the results
        


        return await ApplyFilters(query, filter)
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)

            .ToListAsync();
    }
    private static IQueryable<ItemDetails> ApplyFilters(
        IQueryable<ItemDetails> queryable,
        string? filter
       
        )
    {
        return queryable
            
            .WhereIf(!filter.IsNullOrWhiteSpace(),
            x => x.SKU.Contains(filter)||x.ItemName.Contains(filter)
            || (x.StockOnHand != null && x.StockOnHand.ToString().Contains(filter))
            || (x.SaleableQuantity != null && x.SaleableQuantity.ToString().Contains(filter))
            || (x.PreOrderableQuantity != null && x.PreOrderableQuantity.ToString().Contains(filter))
            || (x.SaleablePreOrderQuantity != null && x.SaleablePreOrderQuantity.ToString().Contains(filter))
            );
    }
}