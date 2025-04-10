using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Orders.Services;
public class OrderTransactionManager(IOrderTransactionRepository orderTransactionRepository) : DomainService
{
    public async Task<OrderTransaction> CreateAsync(OrderTransaction orderTransaction)
    {
        await orderTransactionRepository.InsertAsync(orderTransaction);
        return orderTransaction;
    }
}