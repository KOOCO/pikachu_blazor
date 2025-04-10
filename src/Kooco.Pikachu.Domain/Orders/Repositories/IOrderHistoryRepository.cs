using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders.Repositories;
public interface IOrderHistoryRepository : IRepository<OrderHistory, Guid>
{
    /// <summary>
    /// Retrieves all history records for a specific order.
    /// </summary>
    Task<List<OrderHistory>> GetAllHistoryByOrderIdAsync(Guid orderId);
}