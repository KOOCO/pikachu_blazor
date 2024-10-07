using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Orders
{
    public class EfCoreOrderMessageRepository : EfCoreRepository<PikachuDbContext, OrderMessage, Guid>, IOrderMessageRepository
    {


        public EfCoreOrderMessageRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
                : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(
            string? filter = null,
            Guid? orderId = null,
            Guid? senderId = null,
            bool? isMerchant = null,
            DateTime? timeStamp = null)
        {
            var query = await ApplyFilterAsync(filter, orderId, senderId, isMerchant, timeStamp);

            return await query.LongCountAsync();
        }

        public async Task<List<OrderMessage>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string? filter = null,
            Guid? orderId = null,
            Guid? senderId = null,
            bool? isMerchant = null,
            DateTime? timeStamp = null)
        {
            var query = await ApplyFilterAsync(filter, orderId, senderId, isMerchant, timeStamp);

            // Apply sorting and pagination
            query = query.OrderBy(sorting ?? nameof(OrderMessage.Timestamp));
            return await query.Skip(skipCount).Take(maxResultCount).ToListAsync();
        }

        private async Task<IQueryable<OrderMessage>> ApplyFilterAsync(
            string? filter,
            Guid? orderId,
            Guid? senderId,
            bool? isMerchant,
            DateTime? timeStamp)
        {

            var query = await GetQueryableAsync();

            // Apply filters using WhereIf
            query = query
                .WhereIf(!string.IsNullOrEmpty(filter), x => x.Message.Contains(filter))
                .WhereIf(orderId.HasValue, x => x.OrderId == orderId.Value)
                .WhereIf(senderId.HasValue, x => x.SenderId == senderId.Value)
                .WhereIf(isMerchant.HasValue, x => x.IsMerchant == isMerchant.Value)
                .WhereIf(timeStamp.HasValue, x => x.Timestamp >= timeStamp.Value);

            return query;
        }


    }
}