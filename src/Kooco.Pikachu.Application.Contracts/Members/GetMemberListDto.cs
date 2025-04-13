using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class GetMemberListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? MemberType { get; set; }
    public IEnumerable<string> SelectedMemberTags { get; set; } = [];
    public DateTime? MinCreationTime { get; set; }
    public DateTime? MaxCreationTime { get; set; }
    public int? MinOrderCount { get; set; }
    public int? MaxOrderCount { get; set; }
    public int? MinSpent { get; set; }
    public int? MaxSpent { get; set; }
}