using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.GroupPurchaseOverviews;

public class GroupPurchaseOverview : Entity<Guid>
{
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? SubTitle { get; set; }
    public string? BodyText { get; set; }
    public bool IsButtonEnable { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonLink { get; set; }
    public int? ModuleNumber { get; set; }
}
