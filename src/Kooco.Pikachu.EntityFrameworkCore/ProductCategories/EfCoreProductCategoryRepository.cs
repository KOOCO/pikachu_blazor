using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.ProductCategories;

public class EfCoreProductCategoryRepository : EfCoreRepository<PikachuDbContext, ProductCategory, Guid>, IProductCategoryRepository
{
    public EfCoreProductCategoryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> GetCountAsync(string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable.LongCountAsync();
    }

    public async Task<List<ProductCategory>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<ProductCategory>> GetFilteredQueryableAsync(string? filter = null)
    {
        var queryable = await GetQueryableAsync();
        return queryable
            .WhereIf(!filter.IsNullOrEmpty(), x => x.Name.Contains(filter) || x.Description.Contains(filter));
    }
}
