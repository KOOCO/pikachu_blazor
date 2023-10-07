using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderRepository: IRepository<Order, Guid>
    {
        Task<long> CountAsync(string? filter);
        Task<List<Order>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string? sorting,
            string? filter
            );
    }
}
