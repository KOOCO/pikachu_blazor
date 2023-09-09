using System;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Items;

public class SetItemRepository : EfCoreRepository<PikachuDbContext, SetItem, Guid>, ISetItemRepository
{
    public SetItemRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<SetItem>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
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
}