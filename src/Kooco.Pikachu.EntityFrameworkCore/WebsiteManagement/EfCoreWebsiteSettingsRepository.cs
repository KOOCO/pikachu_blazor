using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.WebsiteManagement;

public class EfCoreWebsiteSettingsRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, WebsiteSettings, Guid>(dbContextProvider), IWebsiteSettingsRepository
{
    public async Task<WebsiteSettings> GetWithDetailsAsync(Guid id)
    {
        return await (await GetQueryableAsync())
            .Where(ws => ws.Id == id)
            .Include(ws => ws.Modules)
                .ThenInclude(m => m.ModuleItems)
                    .ThenInclude(mi => mi.Item)
            .Include(ws => ws.Modules)
                .ThenInclude(m => m.ModuleItems)
                    .ThenInclude(mi => mi.SetItem)
            .Include(ws => ws.OverviewModules)
            .Include(ws => ws.InstructionModules)
            .Include(ws => ws.ProductRankingModules)
            .Include(ws => ws.Modules)
            .FirstOrDefaultAsync()
            ?? throw new EntityNotFoundException(typeof(WebsiteSettings), id);
    }

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

    public async Task<List<WebsiteSettingsModule>> GetModulesAsync(Guid id)
    {
        var dbContext = await GetDbContextAsync();

        var modules = await dbContext.WebsiteSettings
            .Where(ws => ws.Id == id)
            .Include(ws => ws.Modules.OrderBy(m => m.SortOrder))
                .ThenInclude(m => m.ModuleItems.OrderBy(mi => mi.SortOrder))
                .ThenInclude(mi => mi.Item)
                .ThenInclude(i => i.ItemDetails)
            .Include(ws => ws.Modules.OrderBy(m => m.SortOrder))
                .ThenInclude(m => m.ModuleItems.OrderBy(mi => mi.SortOrder))
                .ThenInclude(mi => mi.Item)
                .ThenInclude(i => i.Images.OrderBy(i => i.SortNo))
            .Include(ws => ws.Modules.OrderBy(m => m.SortOrder))
                .ThenInclude(m => m.ModuleItems.OrderBy(mi => mi.SortOrder))
                .ThenInclude(mi => mi.SetItem)
                .ThenInclude(i => i.SetItemDetails)
            .Include(ws => ws.Modules)
                .ThenInclude(m => m.ModuleItems.OrderBy(mi => mi.SortOrder))
                .ThenInclude(mi => mi.SetItem)
                .ThenInclude(i => i.Images.OrderBy(i => i.SortNo))
            .Include(ws => ws.Modules.OrderBy(m => m.SortOrder))
            .SelectMany(ws => ws.Modules)
            .ToListAsync();

        return modules;
    }

    public async Task<WebsiteSettingsModule> GetModuleAsync(Guid moduleId)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.WebsiteSettingsModules
            .Where(x => x.Id == moduleId)
            .Include(m => m.ModuleItems)
                .ThenInclude(mi => mi.SetItem)
                .ThenInclude(s => s.Images)
            .Include(m => m.ModuleItems)
                .ThenInclude(mi => mi.Item)
                .ThenInclude(i => i.Images)
            .Include(m => m.ModuleItems)
                .ThenInclude(mi => mi.Item)
                .ThenInclude(i => i.ItemDetails)
            .FirstOrDefaultAsync();
    }
}
