using System;

namespace Kooco.Pikachu.Campaigns;

public class CreateCampaignAddOnProductDto
{
    public Guid? AddOnProductId { get; set; }
    public int? AddOnProductAmount { get; set; }
    public int? AddOnLimitPerOrder { get; set; }
    public bool? IsUnlimitedQuantity { get; set; }
    public int? AvailableQuantity { get; set; }
    public AddOnDisplayPrice? AddOnDisplayPrice { get; set; }
    public AddOnProductCondition? AddOnProductCondition { get; set; }
    public int? Threshold { get; set; }
}