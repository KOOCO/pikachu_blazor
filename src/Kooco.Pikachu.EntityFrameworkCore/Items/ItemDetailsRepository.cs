using System;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

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
}