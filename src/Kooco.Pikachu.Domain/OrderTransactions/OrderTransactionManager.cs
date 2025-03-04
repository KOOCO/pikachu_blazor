using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.OrderTransactions;

public class OrderTransactionManager(IOrderTransactionRepository orderTransactionRepository) : DomainService
{
    private readonly IOrderTransactionRepository _orderTransactionRepository = orderTransactionRepository;

    public async Task<OrderTransaction> CreateAsync(OrderTransaction orderTransaction)
    {
        await _orderTransactionRepository.InsertAsync(orderTransaction);
        return orderTransaction;
    }
}
