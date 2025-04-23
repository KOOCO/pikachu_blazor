using Kooco.Pikachu.GroupBuys;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Campaigns;

public class CampaignGroupBuy : Entity<Guid>
{
    public Guid CampaignId { get; set; }
    public Guid GroupBuyId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    [ForeignKey(nameof(GroupBuyId))]
    public virtual GroupBuy GroupBuy { get; set; }

    public CampaignGroupBuy(
        Guid id,
        Guid campaignId,
        Guid groupBuyId
        ) : base(id)
    {
        CampaignId = campaignId;
        GroupBuyId = groupBuyId;
    }
}
