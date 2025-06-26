using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Campaigns;

public class Campaign : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Name { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? Description { get; private set; }
    public string? TargetAudienceJson { get; private set; }
    public PromotionModule PromotionModule { get; set; }
    public bool ApplyToAllGroupBuys { get; set; }
    public bool? ApplyToAllProducts { get; private set; }
    public CampaignUsagePolicy DiscountUsagePolicy { get; internal set; }
    public CampaignUsagePolicy ShoppingCreditUsagePolicy { get; internal set; }
    public CampaignUsagePolicy AddOnProductUsagePolicy { get; internal set; }
    public bool IsEnabled { get; set; }
    public Guid? TenantId { get; set; }
    public virtual ICollection<CampaignGroupBuy> GroupBuys { get; set; } = [];
    public virtual ICollection<CampaignProduct> Products { get; set; } = [];
    public virtual CampaignDiscount Discount { get; internal set; }
    public virtual CampaignShoppingCredit ShoppingCredit { get; internal set; }
    public virtual CampaignAddOnProduct AddOnProduct { get; internal set; }
    public virtual ICollection<UseableCampaign> UseableCampaigns { get; set; } = [];

    [NotMapped]
    public IEnumerable<string> TargetAudience =>
        !string.IsNullOrWhiteSpace(TargetAudienceJson)
                ? JsonSerializer.Deserialize<List<string>>(TargetAudienceJson)!
                : [];

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

    private Campaign() { }

    internal Campaign(
        Guid id,
        string name,
        DateTime startDate,
        DateTime endDate,
        string? description,
        IEnumerable<string> targetAudience,
        PromotionModule promotionModule,
        bool applyToAllGroupBuys,
        bool? applyToAllProducts
        ) : base(id)
    {
        SetName(name);
        SetDateRange(startDate, endDate);
        SetDescription(description);
        SetTargetAudience(targetAudience);
        PromotionModule = promotionModule;
        ApplyToAllGroupBuys = applyToAllGroupBuys;
        SetApplyToAllProducts(applyToAllProducts);
        SetIsEnabled(true);
        GroupBuys = new List<CampaignGroupBuy>();
        Products = new List<CampaignProduct>();
    }

    public void SetName(string name) => Name = Check.NotNullOrWhiteSpace(name, nameof(Name), CampaignConsts.MaxNameLength);
    public void SetDescription(string? description) => Description = Check.Length(description, nameof(Description), CampaignConsts.MaxDescriptionLength);
    public void SetDateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new BusinessException("Start Date must be greater than End Date");
        }
        StartDate = Check.NotNull(startDate, nameof(StartDate));
        EndDate = Check.NotNull(endDate, nameof(EndDate));
    }
    public void SetTargetAudience(IEnumerable<string> targetAudience) => TargetAudienceJson = JsonSerializer.Serialize(targetAudience.Distinct());
    public void SetIsEnabled(bool isEnabled) => IsEnabled = isEnabled;
    public void SetApplyToAllProducts(bool? applyToAllProducts)
    {
        if (PromotionModule != PromotionModule.AddOnProduct && !applyToAllProducts.HasValue)
        {
            throw new BusinessException("Apply To All Products is required");
        }

        ApplyToAllProducts = applyToAllProducts;
    }
    public CampaignGroupBuy AddGroupBuy(Guid id, Guid groupBuyId)
    {
        var groupBuy = new CampaignGroupBuy(id, Id, groupBuyId);
        GroupBuys.Add(groupBuy);
        return groupBuy;
    }
    public CampaignProduct AddProduct(Guid id, Guid productId)
    {
        var product = new CampaignProduct(id, Id, productId);
        Products.Add(product);
        return product;
    }
    public UseableCampaign AddAllowedDiscount(Guid id, Guid campaignId)
    {
        return AddUseableCampaign(id, campaignId, PromotionModule.Discount);
    }
    public UseableCampaign AddAllowedShoppingCredit(Guid id, Guid campaignId)
    {
        return AddUseableCampaign(id, campaignId, PromotionModule.ShoppingCredit);
    }
    public UseableCampaign AddAllowedAddOnProduct(Guid id, Guid campaignId)
    {
        return AddUseableCampaign(id, campaignId, PromotionModule.AddOnProduct);
    }
    public UseableCampaign AddUseableCampaign(Guid id, Guid campaignId, PromotionModule promotionModule)
    {
        var useableCampaign = new UseableCampaign(id, Id, campaignId, promotionModule);
        UseableCampaigns.Add(useableCampaign);
        return useableCampaign;
    }
}



