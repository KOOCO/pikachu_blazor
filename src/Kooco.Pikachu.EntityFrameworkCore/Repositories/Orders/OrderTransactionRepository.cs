using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.OrderTransactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Repositories.Orders;

public class OrderTransactionRepository : EfCoreRepository<PikachuDbContext, OrderTransaction, Guid>, IOrderTransactionRepository
{
    public OrderTransactionRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> CountAsync(string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable.LongCountAsync();
    }

    public async Task<List<OrderTransaction>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter)
    {
        var queryable = await GetFilteredQueryableAsync(filter);
        return await queryable
            .OrderBy(!string.IsNullOrEmpty(sorting) ? sorting : OrderTransactionConsts.DefaultSorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<IQueryable<OrderTransaction>> GetFilteredQueryableAsync(string? filter)
    {
        var queryable = await GetQueryableAsync();

        return queryable
            .Include(q => q.Order)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.OrderId.ToString() == filter
            || x.OrderNo.Contains(filter) || x.FailedReason != null && x.FailedReason.Contains(filter));
    }
}
