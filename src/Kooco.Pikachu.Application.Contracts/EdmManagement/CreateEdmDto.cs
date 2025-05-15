using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.EdmManagement;

public class CreateEdmDto
{
    public EdmTemplateType? TemplateType { get; set; }
    public Guid? CampaignId { get; set; }
    public EdmMemberType? MemberType { get; set; }
    public IEnumerable<string> MemberTags { get; set; }
    public bool? ApplyToAllGroupBuys { get; set; }
    public Guid? GroupBuyId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SendTime { get; set; }
    public EdmSendFrequency? SendFrequency { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }

    public CreateEdmDto()
    {
        MemberTags = [];
    }
}