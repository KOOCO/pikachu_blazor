using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class GetMemberListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}