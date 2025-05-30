using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.ProductCategories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.OpenIddict;

namespace Kooco.Pikachu.Items;

public class ItemRepository : EfCoreRepository<PikachuDbContext, Item, Guid>, IItemRepository
{
    public ItemRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<Item> FindByNameAsync(string itemName)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.ItemName == itemName);
    }

    public async Task<Item> FindBySKUAsync(string SKU)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.ItemDetails)
            .FirstOrDefaultAsync(x => x.ItemDetails.Any(i => i.SKU == SKU));
    }

    public async Task<IQueryable<Item>> GetWithImagesAsync(int? maxResultCount = null)
    {
        return (await GetQueryableAsync())
            .Include(x => x.Images)
            .TakeIf<Item, IQueryable<Item>>(maxResultCount.HasValue, maxResultCount.Value);
    }

    public async Task DeleteManyAsync(List<Guid> ids)
    {
        var dbContext = await GetDbContextAsync();
        dbContext.Items.RemoveRange(dbContext.Items.Where(item => ids.Contains(item.Id)));
        dbContext.GroupBuyItemGroupDetails.RemoveRange(dbContext.GroupBuyItemGroupDetails.Where(gd => ids.Contains(gd.ItemId ?? Guid.Empty)));
    }
    public async Task<List<ItemWithItemType>> GetItemsAllLookupAsync()
    {
        return await (await GetQueryableAsync())

            .Select(x => new ItemWithItemType
            {
                Id = x.Id,
                Name = x.ItemName,

            }).ToListAsync();
    }

    public async Task<List<ItemWithItemType>> GetItemsLookupAsync()
    {
        return await (await GetQueryableAsync())
            .Where(x => x.IsItemAvaliable)
            .Select(x => new ItemWithItemType
            {
                Id = x.Id,
                Name = x.ItemName,
                ItemType = ItemType.Item
            }).ToListAsync();
    }

    public async Task<List<ItemListViewModel>> GetItemsListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            Guid? itemId = null,
            DateTime? minAvailableTime = null,
            DateTime? maxAvailableTime = null,
            bool? isFreeShipping = null,
            bool? isAvailable = null
            )
    {
        var queryable = await GetQueryableAsync();

        return await ApplyFilters(queryable, itemId, minAvailableTime, maxAvailableTime, isFreeShipping, isAvailable)
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            .Select(item => new ItemListViewModel
            {
                Id = item.Id,
                ItemName = item.ItemName,
                ItemBadge = item.ItemBadge,
                ItemDescriptionTitle = item.ItemDescriptionTitle,
                LimitAvaliableTimeStart = item.LimitAvaliableTimeStart,
                LimitAvaliableTimeEnd = item.LimitAvaliableTimeEnd,
                CreationTime = item.CreationTime,
                ShareProfit = item.ShareProfit,
                IsFreeShipping = item.IsFreeShipping,
                IsItemAvaliable = item.IsItemAvaliable
            }).ToListAsync();
    }

    public async Task<long> LongCountAsync(
        Guid? itemId = null,
        DateTime? minAvailableTime = null,
        DateTime? maxAvailableTime = null,
        bool? isFreeShipping = null,
        bool? isAvailable = null
        )
    {
        return await ApplyFilters(await GetQueryableAsync(), itemId, minAvailableTime, maxAvailableTime, isFreeShipping, isAvailable).LongCountAsync();
    }

    private static IQueryable<Item> ApplyFilters(
        IQueryable<Item> queryable,
        Guid? itemId,
        DateTime? minAvailableTime,
        DateTime? maxAvailableTime,
        bool? isFreeShipping,
        bool? isAvailable
        )
    {
        return queryable
            .WhereIf(itemId.HasValue, x => x.Id == itemId)
            .WhereIf(minAvailableTime.HasValue, x => x.LimitAvaliableTimeStart >= minAvailableTime || x.LimitAvaliableTimeEnd >= minAvailableTime)
            .WhereIf(maxAvailableTime.HasValue, x => x.LimitAvaliableTimeStart <= maxAvailableTime || x.LimitAvaliableTimeEnd <= maxAvailableTime)
            .WhereIf(isFreeShipping.HasValue, x => x.IsFreeShipping == isFreeShipping)
            .WhereIf(isAvailable.HasValue, x => x.IsItemAvaliable == isAvailable);
    }

    public async Task<Item> GetFirstImage(Guid id)
    {
        return (await GetQueryableAsync())
            .Include(x => x.Images)
            .Where(x => x.Id == id)

            .FirstOrDefault();
    }

    public async Task<List<Item>> GetManyAsync(List<Guid> itemIds)
    {
        var queryable = await GetQueryableAsync();

        return await queryable
            .Where(i => itemIds.Contains(i.Id))
            .Include(i => i.Images)
            .ToListAsync();
    }

    public async Task<Item> GetSKUAndItemAsync(Guid itemId, Guid itemDetailId)
    {
        return (await GetQueryableAsync()).Where(w => w.Id == itemId)
                                          .Include(i => i.Images)
                                          .Include(i => i.ItemDetails.Where(w => w.Id == itemDetailId))
                                          .FirstOrDefault();
    }

    public async Task<ItemDetails?> FindItemDetailAsync(Guid itemDetailId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.ItemDetails
            .Where(x => x.Id == itemDetailId).FirstOrDefaultAsync();
    }

    public async Task<List<CategoryProduct>> GetItemCategoriesAsync(Guid id)
    {
        var queryable = await GetQueryableAsync();

        return await queryable
            .Where(x => x.Id == id)
            .Include(x => x.CategoryProducts)
            .ThenInclude(x => x.ProductCategory)
            .ThenInclude(x => x.ProductCategoryImages)
            .SelectMany(x => x.CategoryProducts)
            .ToListAsync();
    }

    public async Task<List<Item>> GetItemsWithAttributesAsync(List<Guid> ids)
    {
        var queryable = await GetQueryableAsync();
        return await queryable
            .Where(x => ids.Contains(x.Id))
            .Include(x => x.ItemDetails)
            .ToListAsync();
    }

    public async Task DeleteItemBadgeAsync(string itemBadge, string? itemBadgeColor)
    {
        var dbContext = await GetDbContextAsync();

        await dbContext.Items
            .Where(item => item.ItemBadge == itemBadge && item.ItemBadgeColor == itemBadgeColor)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.ItemBadge, p => "")
                .SetProperty(p => p.ItemBadgeColor, p => ""));

        await dbContext.SetItems
            .Where(item => item.SetItemBadge == itemBadge && item.SetItemBadgeColor == itemBadgeColor)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.SetItemBadge, p => "")
                .SetProperty(p => p.SetItemBadgeColor, p => ""));
    }

    public async Task<List<Item>> GetItemsWithDetailsByIdsAsync(List<Guid> itemIds)
    {
        var queryable = await GetQueryableAsync();

        return await queryable
            .Where(item => itemIds.Contains(item.Id))
            .Include(item => item.ItemDetails)
            .Include(Item=>Item.CategoryProducts)
            .ThenInclude(x=>x.ProductCategory)
            .Include(Item=>Item.TaxType)
            .ToListAsync();
    }
}