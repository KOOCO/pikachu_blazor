using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Campaigns;

public class UseableCampaign : Entity<Guid>, IMultiTenant
{
    public Guid UseableCampaignGroupId { get; set; }
    public Guid AllowedCampaignId { get; set; }
    public PromotionModule PromotionModule { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(UseableCampaignGroupId))]
    public virtual UseableCampaignGroup UseableCampaignGroup { get; set; }

    [ForeignKey(nameof(AllowedCampaignId))]
    public virtual Campaign AllowedCampaign { get; set; }

    private UseableCampaign() { }

    public UseableCampaign(
        Guid id,
        Guid useableCampaignGroupId,
        Guid allowedCampaignId,
        PromotionModule promotionModule
        ) : base(id)
    {
        UseableCampaignGroupId = useableCampaignGroupId;
        AllowedCampaignId = allowedCampaignId;
        PromotionModule = promotionModule;
    }
}
