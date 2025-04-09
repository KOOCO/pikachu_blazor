using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.OrderTransactions;

public class OrderTransactionAppService(IOrderTransactionRepository orderTransactionRepository) : PikachuAppService, IOrderTransactionAppService
{
    private readonly IOrderTransactionRepository _orderTransactionRepository = orderTransactionRepository;
    
    public async Task<PagedResultDto<OrderTransactionDto>> GetListAsync(GetOrderTransactionListDto input)
    {
        var totalCount = await _orderTransactionRepository.CountAsync(input.Filter);

        var items = await _orderTransactionRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

        return new PagedResultDto<OrderTransactionDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<OrderTransaction>, List<OrderTransactionDto>>(items)
        };
    }
}
