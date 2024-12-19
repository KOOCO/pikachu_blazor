using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.WebsiteManagement;

public class EfCoreWebsiteSettingsRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, WebsiteSettings, Guid>(dbContextProvider), IWebsiteSettingsRepository
{
    public async Task<long> GetCountAsync(string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable.LongCountAsync();
    }

    public async Task<List<WebsiteSettings>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<WebsiteSettings>> GetFilteredQueryableAsync(string? filter)
    {
        var queryable = await GetQueryableAsync();

        return queryable
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.PageTitle.Contains(filter)
            || x.PageLink.Contains(filter) || (x.PageDescription != null && x.PageDescription.Contains(filter)));
    }
}
