using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Campaigns;

public class CampaignStageSetting : Entity<Guid>
{
    public Guid ShoppingCreditId { get; set; }
    public int Spend { get; set; }
    public int PointsToReceive { get; set; }

    [ForeignKey(nameof(ShoppingCreditId))]
    public virtual CampaignShoppingCredit ShoppingCredit { get; set; }

    public CampaignStageSetting(
        Guid id,
        Guid shoppingCreditId,
        int spend,
        int pointsToReceive
        ) : base(id)
    {
        ShoppingCreditId = shoppingCreditId;
        Spend = Check.Range(spend, nameof(Spend), 0);
        PointsToReceive = Check.Range(pointsToReceive, nameof(Spend), 0);
    }
}
