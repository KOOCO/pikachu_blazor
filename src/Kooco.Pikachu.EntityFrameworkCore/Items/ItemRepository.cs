using System;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Items;

public class ItemRepository : EfCoreRepository<PikachuDbContext, Item, Guid>, IItemRepository
{
    public ItemRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<Item>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}