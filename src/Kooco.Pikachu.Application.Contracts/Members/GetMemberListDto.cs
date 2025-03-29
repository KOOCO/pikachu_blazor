using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class GetMemberListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? MemberType { get; set; }
    public IEnumerable<string> SelectedMemberTags { get; set; } = [];
}