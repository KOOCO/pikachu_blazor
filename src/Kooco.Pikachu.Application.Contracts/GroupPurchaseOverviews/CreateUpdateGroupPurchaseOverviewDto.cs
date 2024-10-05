using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupPurchaseOverviews;

public class CreateUpdateGroupPurchaseOverviewDto
{
    public Guid Id { get; set; }
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
