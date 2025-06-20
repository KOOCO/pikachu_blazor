using Kooco.Pikachu.Campaigns;
using System;

namespace Kooco.Pikachu.Orders;

public class AppliedCampaignDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid CampaignId { get; set; }
    public PromotionModule Module { get; set; }
    public int Amount { get; set; }
}
