using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderRepository: IRepository<Order, Guid>
    {
        Task<long> CountAsync(string? filter, Guid? groupBuyId);
        Task<List<Order>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string? sorting,
            string? filter,
            Guid? groupBuyId
            );
        Task<Order> MaxByOrderNumberAsync();
        Task<Order> GetWithDetailsAsync(Guid id);
    }
}
