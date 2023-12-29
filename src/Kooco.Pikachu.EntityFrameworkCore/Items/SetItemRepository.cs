using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Items;

public class SetItemRepository : EfCoreRepository<PikachuDbContext, SetItem, Guid>, ISetItemRepository
{
    public SetItemRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<SetItem> GetWithDetailsAsync(Guid id)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.SetItems
        .Include(s => s.Images)
        .Include(s => s.SetItemDetails)
            .ThenInclude(d => d.Item)
            .ThenInclude(i => i.Images)
        .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task DeleteManyAsync(List<Guid> ids)
    {
        var dbContext = await GetDbContextAsync();
        dbContext.SetItems.RemoveRange(dbContext.SetItems.Where(setItem => ids.Contains(setItem.Id)));
        dbContext.GroupBuyItemGroupDetails.RemoveRange(dbContext.GroupBuyItemGroupDetails.Where(gd => ids.Contains(gd.SetItemId ?? Guid.Empty)));
    }

    public async Task<List<ItemWithItemType>> GetItemsLookupAsync()
    {
        return await (await GetQueryableAsync())
            .Select(x => new ItemWithItemType
            {
                Id = x.Id,
                Name = x.SetItemName,
                ItemType = ItemType.SetItem
            }).ToListAsync();
    }
}