using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
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
}