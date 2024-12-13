using Kooco.Pikachu.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.OrderDeliveries
{
    public interface IOrderDeliveryRepository : IRepository<OrderDelivery, Guid>
    {
        Task<List<OrderDelivery>> GetWithDetailsAsync(Guid id);
        Task<Guid> GetOrderIdByAllPayLogisticsId(string AllPayLogisticsId);
    }
}
