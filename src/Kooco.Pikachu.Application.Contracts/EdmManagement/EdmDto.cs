using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.EdmManagement;

public class EdmDto : FullAuditedEntityDto<Guid>, IMultiTenant
{
    public EdmTemplateType TemplateType { get; set; }
    public Guid? CampaignId { get; set; }
    public string? CampaignName { get; set; }
    public bool ApplyToAllMembers { get; set; }
    public Guid GroupBuyId { get; set; }
    public string? GroupBuyName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime SendTime { get; set; }
    public EdmSendFrequency? SendFrequency { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public Guid? TenantId { get; set; }
    public IEnumerable<string> MemberTags { get; set; }
}
