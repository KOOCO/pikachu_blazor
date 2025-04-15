using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members.MemberTags;

public class GetMemberTagsListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
