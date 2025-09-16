using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TenantPayouts;

public class GetTenantSummariesDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}