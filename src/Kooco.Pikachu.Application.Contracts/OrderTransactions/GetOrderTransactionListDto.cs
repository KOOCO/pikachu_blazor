using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.OrderTransactions;

public class GetOrderTransactionListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}