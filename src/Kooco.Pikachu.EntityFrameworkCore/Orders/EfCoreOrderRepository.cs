using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
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
        public async Task<long> CountAsync(string? filter, Guid? groupBuyId)
        {
            return await ApplyFilters((await GetQueryableAsync()).Include(o => o.GroupBuy), filter, groupBuyId).CountAsync();
        }
        public async Task<List<Order>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId)
        {
            return await ApplyFilters(await GetQueryableAsync(), filter, groupBuyId)
                .OrderBy(sorting)
                .PageBy(skipCount, maxResultCount)
                .Include(o => o.GroupBuy)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.SetItem)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Freebie)
                .ToListAsync();
        }
        private static IQueryable<Order> ApplyFilters(
            IQueryable<Order> queryable,
            string? filter,
            Guid? groupBuyId
            )
        {
            return queryable
                .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                x => x.OrderNo.Contains(filter)
                || (x.CustomerName != null && x.CustomerName.Contains(filter))
                || (x.CustomerEmail != null && x.CustomerEmail.Contains(filter))
                );
        }
        public async Task<Order> MaxByOrderNumberAsync()
        {
            var orders = await (await GetQueryableAsync()).ToListAsync();
            return orders.OrderByDescending(x => long.Parse(x.OrderNo[^9..])).FirstOrDefault();
        }
        public async Task<Order> GetWithDetailsAsync(Guid id)
        {
            return await (await GetQueryableAsync())
                .Where(o => o.Id == id)
                .Include(o => o.GroupBuy)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .ThenInclude(i => i.Images)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.SetItem)
                    .ThenInclude(i => i.Images)
                .Include(o => o.OrderItems.OrderBy(oi => oi.ItemType))
                    .ThenInclude(oi => oi.Freebie)
                    .ThenInclude(i => i.Images)
                .Include(o => o.StoreComments.OrderByDescending(c => c.CreationTime))
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();
        }
    }
}
