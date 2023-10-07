using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class GetOrderListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
