using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Orders;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Kooco.Pikachu.Items;

public class ItemDetailsRepository : EfCoreRepository<PikachuDbContext, ItemDetails, Guid>, IItemDetailsRepository
{
    public ItemDetailsRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<ItemDetails>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
    public async Task<long> CountAsync(string? filter)
    {
        return await ApplyFilters((await GetQueryableAsync()), filter).CountAsync();
    }
    public async Task<List<ItemDetails>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter)
    {
     

        return await ApplyFilters(await GetQueryableAsync(), filter)
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            
            .ToListAsync();
    }
    private static IQueryable<ItemDetails> ApplyFilters(
        IQueryable<ItemDetails> queryable,
        string? filter
       
        )
    {
        return queryable
            
            .WhereIf(!filter.IsNullOrWhiteSpace(),
            x => x.SKU.Contains(filter)
            || (x.StockOnHand != null && x.StockOnHand.ToString().Contains(filter))
            || (x.SaleableQuantity != null && x.SaleableQuantity.ToString().Contains(filter))
            || (x.PreOrderableQuantity != null && x.PreOrderableQuantity.ToString().Contains(filter))
            || (x.SaleablePreOrderQuantity != null && x.SaleablePreOrderQuantity.ToString().Contains(filter))
            );
    }
}