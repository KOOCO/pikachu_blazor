using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders.Repositories;
public interface IOrderDeliveryRepository : IRepository<OrderDelivery, Guid>
{
    Task<List<OrderDelivery>> GetWithDetailsAsync(Guid id);
    Task<Guid> GetOrderIdByAllPayLogisticsId(string AllPayLogisticsId);
}