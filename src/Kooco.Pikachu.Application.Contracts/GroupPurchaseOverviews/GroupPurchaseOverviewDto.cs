using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.GroupPurchaseOverviews;

public class GroupPurchaseOverviewDto : AuditedEntityDto<Guid>
{
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? SubTitle { get; set; }
    public string? BodyText { get; set; }
    public bool IsButtonEnable { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonLink { get; set; }
    public bool IsDeleted { get; set; }
}
