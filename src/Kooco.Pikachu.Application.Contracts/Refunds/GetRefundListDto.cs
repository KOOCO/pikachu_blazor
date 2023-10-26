using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Refunds
{
    public class GetRefundListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
