using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderRepository: IRepository<Order, Guid>
    {
        Task<long> CountAsync(string? filter, Guid? groupBuyId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<Order>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string? sorting,
            string? filter,
            Guid? groupBuyId,
            List<Guid>orderId, DateTime? startDate = null, DateTime? endDate = null
            );
        Task<Order> MaxByOrderNumberAsync();
        Task<Order> GetWithDetailsAsync(Guid id);
        Task<long> ReturnOrderCountAsync(string? filter, Guid? groupBuyId);
        Task<List<Order>> GetReturnListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId);
        Task<long> CountReconciliationAsync(string? filter, Guid? groupBuyId, DateTime? startDate, DateTime? endDate);
        Task<List<Order>> GetReconciliationListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId, List<Guid> orderId, DateTime? startDate = null, DateTime? endDate = null);

    }
}
