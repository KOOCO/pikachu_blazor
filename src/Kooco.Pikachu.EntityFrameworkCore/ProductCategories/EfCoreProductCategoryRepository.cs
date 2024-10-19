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
        return await queryable
            .OrderBy(sorting.IsNullOrWhiteSpace() ? ProductCategoryConsts.DefaultSorting : sorting)
            .PageBy(skipCount, maxResultCount)
            .Include(x => x.ProductCategoryImages)
            .ToListAsync();
    }

    public async Task<IQueryable<ProductCategory>> GetFilteredQueryableAsync(string? filter = null)
    {
        var queryable = await GetQueryableAsync();
        return queryable
            .WhereIf(!filter.IsNullOrEmpty(), x => x.Name.Contains(filter) || (x.Description != null && x.Description.Contains(filter)));
    }

    public async Task<ProductCategory?> FindByNameAsync(string name)
    {
        var queryable = await GetQueryableAsync();
        return await queryable
            .Where(category => name.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefaultAsync();
    }

    public async Task<ProductCategory> GetWithDetailsAsync(Guid id, bool includeItem = false)
    {
        var queryable = await GetQueryableAsync();
        queryable = queryable
            .Where(category => category.Id == id)
            .Include(category => category.ProductCategoryImages);

        if (includeItem)
        {
            queryable = queryable
                .Include(category => category.CategoryProducts)
                .ThenInclude(product => product.Item);
        }
        else
        {
            queryable = queryable
                .Include(category => category.CategoryProducts);
        }

        return await queryable.FirstOrDefaultAsync();
    }
}
