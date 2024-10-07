using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderMessageRepository : IRepository<OrderMessage, Guid>
    {
        // Get the count of messages with optional filters
        Task<long> GetCountAsync(
            string? filter=null,
            Guid? orderId = null,
            Guid? senderId = null,
            bool? isMerchant = null,
            DateTime? timeStamp = null
           
        );

       

        // Get a paginated list of messages with optional filters and sorting
        Task<List<OrderMessage>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
             string? filter = null,
            Guid? orderId = null,
            Guid? senderId = null,
            bool? isMerchant = null,
            DateTime? timeStamp = null
        );
    }
}