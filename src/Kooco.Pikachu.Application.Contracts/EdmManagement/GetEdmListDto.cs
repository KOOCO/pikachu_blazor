using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.EdmManagement;

public class GetEdmListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public EdmTemplateType? TemplateType { get; set; }
    public Guid? CampaignId { get; set; }
    public EdmMemberType? MemberType { get; set; }
    public IEnumerable<string> MemberTags { get; set; } = [];
    public bool? ApplyToAllGroupBuys { get; set; }
    public IEnumerable<Guid> GroupBuyIds { get; set; } = [];
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? MinSendTime { get; set; }
    public DateTime? MaxSendTime { get; set; }
    public EdmSendFrequency? SendFrequency { get; set; }
}
