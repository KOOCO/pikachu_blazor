using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Campaigns;

public class CreateCampaignDto
{
    public string Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> TargetAudience { get; set; } = [];
    public PromotionModule? PromotionModule { get; set; }
    public bool? ApplyToAllGroupBuys { get; set; }
    public IEnumerable<Guid> GroupBuyIds { get; set; } = [];
    public bool? ApplyToAllProducts { get; set; }
    public IEnumerable<Guid> ProductIds { get; set; } = [];

    public CreateCampaignDiscountDto Discount { get; set; } = new();
    public CreateCampaignShoppingCreditDto ShoppingCredit { get; set; } = new();
    public CreateCampaignAddOnProductDto AddOnProduct { get; set; } = new();
}