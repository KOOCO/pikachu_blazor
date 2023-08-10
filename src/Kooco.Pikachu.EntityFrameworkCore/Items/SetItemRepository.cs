using System;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
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
}