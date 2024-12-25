using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class EfCoreFooterSettingRepository : EfCoreRepository<PikachuDbContext, FooterSetting, Guid>, IFooterSettingRepository
{
    public EfCoreFooterSettingRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<FooterSetting?> FirstOrDefaultAsync()
    {
        var queryable = await GetQueryableAsync();
        return await queryable
            .Include(x => x.Sections.OrderBy(s => s.FooterSettingsPosition))
            .ThenInclude(x => x.Links.OrderBy(l => l.Index))
            .FirstOrDefaultAsync();
    }
}
