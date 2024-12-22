using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
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
    public async Task<long> GetCountAsync(string? filter, string? pageTitle, WebsitePageType? pageType, Guid? productCategoryId,
        GroupBuyTemplateType? templateType, bool? setAsHomePage)
    {
        var queryable = await GetFilteredQueryableAsync(filter, pageTitle, pageType, productCategoryId, templateType, setAsHomePage);
        return await queryable.LongCountAsync();
    }

    public async Task<List<WebsiteSettings>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter, string? pageTitle,
        WebsitePageType? pageType, Guid? productCategoryId, GroupBuyTemplateType? templateType, bool? setAsHomePage)
    {
        var queryable = await GetFilteredQueryableAsync(filter, pageTitle, pageType, productCategoryId, templateType, setAsHomePage);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<WebsiteSettings>> GetFilteredQueryableAsync(string? filter, string? pageTitle, WebsitePageType? pageType,
        Guid? productCategoryId, GroupBuyTemplateType? templateType, bool? setAsHomePage)
    {
        var queryable = await GetQueryableAsync();

        return queryable
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.PageTitle.Contains(filter)
            || x.PageLink.Contains(filter) || (x.PageDescription != null && x.PageDescription.Contains(filter))
            || x.Id.ToString().Contains(filter))
            .WhereIf(!pageTitle.IsNullOrWhiteSpace(), x => x.PageTitle == pageTitle)
            .WhereIf(pageType.HasValue, x => x.PageType == pageType)
            .WhereIf(productCategoryId.HasValue, x => x.ProductCategoryId == productCategoryId)
            .WhereIf(templateType.HasValue, x => x.TemplateType == templateType)
            .WhereIf(setAsHomePage.HasValue, x => x.SetAsHomePage == setAsHomePage);
    }
}
