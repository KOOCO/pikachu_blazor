using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class EfCoreTopbarSettingRepository : EfCoreRepository<PikachuDbContext, TopbarSetting, Guid>, ITopbarSettingRepository
{
    public EfCoreTopbarSettingRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<TopbarSetting?> FirstOrDefaultAsync()
    {
        var queryable = await GetQueryableAsync();

        return await queryable
            .Include(x => x.Links.OrderBy(x => x.Index))
            .ThenInclude(x => x.CategoryOptions.OrderBy(x => x.Index))
            .FirstOrDefaultAsync();
    }
}
