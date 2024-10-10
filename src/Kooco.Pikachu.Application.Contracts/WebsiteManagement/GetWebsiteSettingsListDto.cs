using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement;

public class GetWebsiteSettingsListDto : PagedAndSortedResultRequestDto
{
    public string? Filter {  get; set; }
}