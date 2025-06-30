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

public class EfCoreProductCategoryRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, ProductCategory, Guid>(dbContextProvider), IProductCategoryRepository
{
    public async Task<long> GetCountAsync(string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable.Where(x=>x.MainCategoryId==null).LongCountAsync();
    }

    public async Task<List<ProductCategory>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable.Where(x=>x.MainCategoryId==null)
            .OrderBy(sorting.IsNullOrWhiteSpace() ? ProductCategoryConsts.DefaultSorting : sorting)
            .PageBy(skipCount, maxResultCount)
            .Include(x => x.ProductCategoryImages)
            .ToListAsync();
    }

    public async Task<IQueryable<ProductCategory>> GetFilteredQueryableAsync(string? filter = null)
    {
        var queryable = (await GetQueryableAsync()).Where(x=>x.MainCategoryId==null);
        return queryable
            .WhereIf(!filter.IsNullOrEmpty(), x => x.Name.Contains(filter) || (x.Description != null && x.Description.Contains(filter)));
    }

    public async Task<ProductCategory?> FindByNameAsync(string name)
    {
        name = name.ToLowerInvariant();
        var queryable = await GetQueryableAsync();
        return await queryable
            .Where(category => name == category.Name.ToLower())
            .FirstOrDefaultAsync();
    }
    public async Task<ProductCategory?> FindByZhNameAsync(string name)
    {
        name = name.ToLowerInvariant();
        var queryable = await GetQueryableAsync();
        return await queryable
            .Where(category => name == category.ZHName.ToLower())
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

    public async Task<string?> GetDefaultImageUrlAsync(Guid id)
    {
        var productImage = await (await GetQueryableAsync())
            .Where(x => x.Id == id)
            .SelectMany(x => x.ProductCategoryImages)
            .OrderBy(i => i.SortNo)
            .Where(i => i.Url != null)
            .FirstOrDefaultAsync();

        return productImage?.Url;
    }
}
