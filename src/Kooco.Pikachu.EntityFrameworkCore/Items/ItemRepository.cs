using System;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
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

    public override async Task<IQueryable<Item>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}