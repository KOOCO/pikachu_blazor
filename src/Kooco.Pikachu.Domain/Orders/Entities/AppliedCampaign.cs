using Kooco.Pikachu.Campaigns;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Orders.Entities;

public class AppliedCampaign : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid CampaignId { get; set; }
    public PromotionModule Module { get; set; }
    public int Amount { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    private AppliedCampaign() { }

    public AppliedCampaign(
        Guid id,
        Guid orderId,
        Guid campaignId,
        PromotionModule module,
        int amount
        ) : base(id)
    {
        OrderId = orderId;
        CampaignId = campaignId;
        Module = module;
        Amount = amount;
    }
}
