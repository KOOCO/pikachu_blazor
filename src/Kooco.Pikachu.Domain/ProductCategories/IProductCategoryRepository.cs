using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ProductCategories;

public interface IProductCategoryRepository : IRepository<ProductCategory, Guid>
{
    Task<long> GetCountAsync(string? filter);
    Task<List<ProductCategory>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter);
    Task<IQueryable<ProductCategory>> GetFilteredQueryableAsync(string? filter = null);
    Task<ProductCategory?> FindByNameAsync(string name);
    Task<ProductCategory> GetWithDetailsAsync(Guid id, bool includeItem = false);
    Task<string?> GetDefaultImageUrlAsync(Guid id);
}
