using System;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Items;

public class SetItemDetailsRepository : EfCoreRepository<PikachuDbContext, SetItemDetails, Guid>, ISetItemDetailsRepository
{
    public SetItemDetailsRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<SetItemDetails>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}