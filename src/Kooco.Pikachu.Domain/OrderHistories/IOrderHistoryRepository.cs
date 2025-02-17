using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.OrderHistories
{
    public interface IOrderHistoryRepository : IRepository<OrderHistory, Guid>
    {
        /// <summary>
        /// Retrieves all history records for a specific order.
        /// </summary>
        Task<List<OrderHistory>> GetAllHistoryByOrderIdAsync(Guid orderId);
    }
}
