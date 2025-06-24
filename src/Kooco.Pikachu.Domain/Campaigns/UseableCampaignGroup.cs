using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Campaigns;

public class UseableCampaignGroup : Entity<Guid>, IMultiTenant
{
    public Guid CampaignId { get; set; }
    public bool UseableWithAllDiscounts { get; internal set; }
    public bool UseableWithAllShoppingCredits { get; internal set; }
    public bool UseableWithAllAddOnProducts { get; internal set; }
    public Guid? TenantId { get; set; }
    public virtual ICollection<UseableCampaign> UseableCampaigns { get; set; } = [];

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    [NotMapped]
    public virtual IEnumerable<UseableCampaign> AllowedDiscounts =>
        UseableCampaigns
            .Where(x => x.PromotionModule == PromotionModule.Discount);

    [NotMapped]
    public virtual IEnumerable<UseableCampaign> AllowedShoppingCredits =>
        UseableCampaigns
            .Where(x => x.PromotionModule == PromotionModule.ShoppingCredit);

    [NotMapped]
    public virtual IEnumerable<UseableCampaign> AllowedAddOnProducts =>
        UseableCampaigns
            .Where(x => x.PromotionModule == PromotionModule.AddOnProduct);

    private UseableCampaignGroup() { }

    public UseableCampaignGroup(
        Guid id,
        Guid campaignId,
        bool useableWithAllDiscounts,
        bool useableWithAllShoppingCredits,
        bool useableWithAllAddOnProducts
        ) : base(id)
    {
        CampaignId = campaignId;
        UseableWithAllDiscounts = useableWithAllDiscounts;
        UseableWithAllShoppingCredits = useableWithAllShoppingCredits;
        UseableWithAllAddOnProducts = useableWithAllAddOnProducts;
    }

    public UseableCampaign AddAllowedDiscount(Guid id, Guid campaignId)
    {
        var allowedDiscount = new UseableCampaign(id, Id, campaignId, PromotionModule.Discount);
        UseableCampaigns.Add(allowedDiscount);
        return allowedDiscount;
    }
    public UseableCampaign AddAllowedShoppingCredit(Guid id, Guid campaignId)
    {
        var allowedShoppingCredit = new UseableCampaign(id, Id, campaignId, PromotionModule.ShoppingCredit);
        UseableCampaigns.Add(allowedShoppingCredit);
        return allowedShoppingCredit;
    }
    public UseableCampaign AddAllowedAddOnProduct(Guid id, Guid campaignId)
    {
        var allowedAddOnProduct = new UseableCampaign(id, Id, campaignId, PromotionModule.AddOnProduct);
        UseableCampaigns.Add(allowedAddOnProduct);
        return allowedAddOnProduct;
    }
}
