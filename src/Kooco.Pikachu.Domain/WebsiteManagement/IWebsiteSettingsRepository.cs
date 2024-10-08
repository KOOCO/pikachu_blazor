using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.WebsiteManagement;

public interface IWebsiteSettingsRepository : IRepository<WebsiteSettings, Guid>
{
    Task<long> GetCountAsync(string? filter);
    Task<List<WebsiteSettings>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter);
    Task<IQueryable<WebsiteSettings>> GetFilteredQueryableAsync(string? filter);
}
