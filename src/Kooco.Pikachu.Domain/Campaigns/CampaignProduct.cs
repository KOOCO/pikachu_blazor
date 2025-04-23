using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Campaigns;

public class CampaignProduct : Entity<Guid>
{
    public Guid CampaignId { get; set; }
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Item Product { get; set; }

    public CampaignProduct(
        Guid id,
        Guid campaignId,
        Guid productId
        ) : base(id)
    {
        CampaignId = campaignId;
        ProductId = productId;
    }
}
