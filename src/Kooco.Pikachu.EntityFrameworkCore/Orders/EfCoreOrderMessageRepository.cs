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
using Volo.Abp.Users;

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
           
            var joinedQuery = from orderMessage in query
                              join user in (await GetDbContextAsync()).Users on orderMessage.SenderId equals user.Id into userGroup
                              from user in userGroup.DefaultIfEmpty() // Left join
                              select new
                              {
                                  OrderMessage = orderMessage,
                                  SenderName = user != null ? user.Email : null
                              };
           
            var resultQuery= joinedQuery.OrderBy("OrderMessage."+sorting).Skip(skipCount).Take(maxResultCount);
            var resultList = await resultQuery.ToListAsync();

            // Map the result to OrderMessageDto
            var orderMessageDtos = resultList.Select(x => new OrderMessage
            {
               
                OrderId = x.OrderMessage.OrderId,
                SenderId = x.OrderMessage.SenderId,
                Message = x.OrderMessage.Message,
                Timestamp = x.OrderMessage.Timestamp,
                IsMerchant = x.OrderMessage.IsMerchant,
                
                SenderName = x.SenderName // Set the sender name
            }).ToList();

            return orderMessageDtos;
        }

        public async Task<List<OrderMessage>> GetOrderMessagesAsync(Guid orderId)
        {
            return [.. (await GetQueryableAsync()).Where(w => w.OrderId == orderId)];
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