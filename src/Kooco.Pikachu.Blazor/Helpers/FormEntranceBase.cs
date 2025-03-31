using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Helpers;
public class FormEntranceBase : PagedAndSortedResultRequestDto
{
    public string? SearchTerm { get; set; }
}