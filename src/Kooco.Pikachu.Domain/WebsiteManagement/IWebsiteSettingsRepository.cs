using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.WebsiteManagement;

public interface IWebsiteSettingsRepository : IRepository<WebsiteSettings, Guid>
{
    Task<long> GetCountAsync(string? filter, string? pageTitle, WebsitePageType? pageType, Guid? productCategoryId,
        GroupBuyTemplateType? templateType, bool? setAsHomePage);
    Task<List<WebsiteSettings>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter, string? pageTitle,
        WebsitePageType? pageType, Guid? productCategoryId, GroupBuyTemplateType? templateType, bool? setAsHomePage);
    Task<IQueryable<WebsiteSettings>> GetFilteredQueryableAsync(string? filter, string? pageTitle, WebsitePageType? pageType,
        Guid? productCategoryId, GroupBuyTemplateType? templateType, bool? setAsHomePage);
}
