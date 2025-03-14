using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Refunds
{
    public class EfCoreRefundRepository : EfCoreRepository<PikachuDbContext, Refund, Guid>, IRefundRepository
    {
        public EfCoreRefundRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(string? filter)
        {
            var query = (await GetQueryableAsync()).Include(x => x.Order);
            return await ApplyFilter(query, filter).LongCountAsync();
        }
        public async Task<long> GetRundPendingCountAsync()
        {
            var query = (await GetQueryableAsync()).Include(x => x.Order).Where(x=>x.RefundReview==EnumValues.RefundReviewStatus.PendingReview);
            return await query.LongCountAsync();
        }

        public async Task<List<Refund>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter)
        {
            var query = (await GetQueryableAsync()).Include(x => x.Order);
            return await ApplyFilter(query, filter)
                .OrderBy(sorting)
                .PageBy(skipCount,maxResultCount)
                .ToListAsync();
        }
        public async Task<Refund> GetAsync(Guid Id)
        {
            var query = (await GetQueryableAsync()).Include(x => x.Order).Where(x=>x.Id==Id).FirstOrDefault();
            return query;
        }
        private static IQueryable<Refund> ApplyFilter(
            IQueryable<Refund> queryable,
            string? filter
            )
        {
            return queryable
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                x => x.Order == null || x.Order.OrderNo.Contains(filter)
                || x.Order.CustomerName.Contains(filter)
                || x.Order.TotalAmount.ToString().Contains(filter)
                ).Where(x=>x.Order.IsRefunded==true);
        }
    }
}
