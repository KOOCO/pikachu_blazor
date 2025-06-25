using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Campaigns;

public class UseableCampaign : Entity<Guid>, IMultiTenant
{
    public Guid CampaignId { get; set; }
    public Guid AllowedCampaignId { get; set; }
    public PromotionModule PromotionModule { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    private UseableCampaign() { }

    public UseableCampaign(
        Guid id,
        Guid campaignId,
        Guid allowedCampaignId,
        PromotionModule promotionModule
        ) : base(id)
    {
        CampaignId = campaignId;
        AllowedCampaignId = allowedCampaignId;
        PromotionModule = promotionModule;
    }
}
