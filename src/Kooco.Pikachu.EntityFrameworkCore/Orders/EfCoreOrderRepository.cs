using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Orders
{
    public class EfCoreOrderRepository : EfCoreRepository<PikachuDbContext, Order, Guid>, IOrderRepository
    {
        public EfCoreOrderRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<long> CountAsync(string? filter)
        {
            return await ApplyFilters(await GetQueryableAsync(), filter).CountAsync();
        }

        public async Task<List<Order>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter)
        {
            return await ApplyFilters(await GetQueryableAsync(), filter)
                .OrderBy(sorting)
                .PageBy(skipCount, maxResultCount)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        private static IQueryable<Order> ApplyFilters(
            IQueryable<Order> queryable,
            string? filter
            )
        {
            return queryable
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                x => x.Name.Contains(filter)
                || x.Email.Contains(filter)
                );
        }
    }
}
