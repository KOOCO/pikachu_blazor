using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
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
            return await ApplyFilters((await GetQueryableAsync()).Include(o => o.GroupBuy), filter).CountAsync();
        }

        public async Task<List<Order>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter)
        {
            return await ApplyFilters(await GetQueryableAsync(), filter)
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
        public async Task<List<OrderReport>> GetGroupBuyReport(int skipCount, int maxResultCount, string? sorting)
        {
            var dbContext = await GetDbContextAsync();
            var query = from o in dbContext.Orders
                        join gb in dbContext.GroupBuys
                        on o.GroupBuyId equals gb.Id
                        group new { o.TotalQuantity, o.TotalAmount, o.ShippingStatus, o.GroupBuyId } by gb.GroupBuyName into grouped
                        select new
                        {
                            GroupBuyName = grouped.Key,
                            TotalQuantity = grouped.Sum(x => x.TotalQuantity),
                            TotalAmount = grouped.Sum(x => x.TotalAmount),
                            PaidAmount = grouped.Sum(x => x.ShippingStatus == ShippingStatus.PrepareShipment ? x.TotalAmount : 0)
                        };
            List<OrderReport> orderReports = System.Linq.Enumerable.AsEnumerable(query)
             .Select(a => new OrderReport
               {
            GroupBuyName = a.GroupBuyName,
            TotalQuantity = a.TotalQuantity,
            TotalAmount = a.TotalAmount,
            PaidAmount = a.PaidAmount
             })
                .ToList();
            return orderReports;        
        } 
        private static IQueryable<Order> ApplyFilters(
            IQueryable<Order> queryable,
            string? filter
            )
        {
            return queryable
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
