using System;

namespace Kooco.Pikachu.Campaigns;

public class CreateCampaignAddOnProductDto
{
    public Guid? ProductId { get; set; }
    public Guid? ItemDetailId { get; set; }
    public int? ProductAmount { get; set; }
    public int? LimitPerOrder { get; set; }
    public bool? IsUnlimitedQuantity { get; set; }
    public int? AvailableQuantity { get; set; }
    public AddOnDisplayPrice? DisplayPrice { get; set; }
    public AddOnProductCondition? ProductCondition { get; set; }
    public int? Threshold { get; set; }
}