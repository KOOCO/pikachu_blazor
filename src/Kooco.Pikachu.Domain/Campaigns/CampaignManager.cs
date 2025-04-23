using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Campaigns;

public class CampaignManager(ICampaignRepository campaignRepository) : DomainService
{
    private readonly ICampaignRepository _campaignRepository = campaignRepository;

    public async Task<Campaign> CreateAsync(string name, DateTime startDate, DateTime endDate, string? description,
        IEnumerable<string> targetAudience, PromotionModule promotionModule, bool applyToAllGroupBuys,
        IEnumerable<Guid> groupBuyIds, bool? applyToAllProducts, IEnumerable<Guid> productIds, bool autoSave = true)
    {
        if (!targetAudience.Any())
        {
            throw new BusinessException("Required", nameof(targetAudience));
        }
        if (applyToAllGroupBuys == false && !groupBuyIds.Any())
        {
            throw new BusinessException("Required", nameof(groupBuyIds));
        }
        if (applyToAllProducts == false && !productIds.Any())
        {
            throw new BusinessException("Required", nameof(productIds));
        }

        var campaign = new Campaign(GuidGenerator.Create(), name, startDate, endDate, description, targetAudience,
            promotionModule, applyToAllGroupBuys, applyToAllProducts);

        if (autoSave)
        {
            await _campaignRepository.InsertAsync(campaign);
        }

        AddGroupBuys(campaign, groupBuyIds, false);
        AddProducts(campaign, productIds, false);

        return campaign;
    }

    public void AddGroupBuys(Campaign campaign, IEnumerable<Guid> groupBuyIds, bool throwException = true)
    {
        var canAdd = campaign.ApplyToAllGroupBuys == false;
        if (throwException && !canAdd)
        {
            throw new BusinessException("Group Buys can not be added to this Campaign. Apply to All Group Buys is checked.");
        }
        if (canAdd)
        {
            foreach (var groupBuyId in groupBuyIds)
            {
                AddGroupBuy(campaign, groupBuyId);
            }
        }
    }

    public CampaignGroupBuy AddGroupBuy(Campaign campaign, Guid groupBuyId)
    {
        Check.NotNull(campaign, nameof(Campaign));
        Check.NotDefaultOrNull<Guid>(groupBuyId, nameof(groupBuyId));

        return campaign.AddGroupBuy(GuidGenerator.Create(), groupBuyId);
    }

    public void AddProducts(Campaign campaign, IEnumerable<Guid> productIds, bool throwException = true)
    {
        var canAdd = campaign.ApplyToAllProducts == false && campaign.PromotionModule != PromotionModule.AddOnProduct;
        if (throwException && !canAdd)
        {
            throw new BusinessException("Products can not be added to this Campaign. Either the module is incorrect or Apply to All Products is checked.");
        }
        if (canAdd)
        {
            foreach (var productId in productIds)
            {
                AddProduct(campaign, productId);
            }
        }
    }

    public CampaignProduct AddProduct(Campaign campaign, Guid productId)
    {
        Check.NotNull(campaign, nameof(Campaign));
        Check.NotDefaultOrNull<Guid>(productId, nameof(productId));

        return campaign.AddProduct(GuidGenerator.Create(), productId);
    }

    public CampaignDiscount AddCampaignDiscount(Campaign campaign, bool isDiscountCodeRequired, string? discountCode, int availableQuantity,
        int maximumUsePerPerson, DiscountMethod discountMethod, int? minimumSpendAmount, bool? applyToAllShippingMethods,
        IEnumerable<DeliveryMethod> deliveryMethods, DiscountType discountType, int? discountAmount, int? discountPercentage)
    {
        Check.NotNull(campaign, nameof(Campaign));
        EnsurePromotionModule(campaign, PromotionModule.Discount);

        campaign.Discount = new CampaignDiscount(GuidGenerator.Create(), campaign.Id, isDiscountCodeRequired, discountCode,
            availableQuantity, maximumUsePerPerson, discountMethod, minimumSpendAmount, applyToAllShippingMethods,
            deliveryMethods, discountType, discountAmount, discountPercentage);

        return campaign.Discount;
    }

    public CampaignShoppingCredit AddShoppingCredit(Campaign campaign, bool canExpire, int? validForDays,
        CalculationMethod calculationMethod, double? calculationPercentage, ApplicableItem applicableItem, int budget,
        List<(int spend, int pointsToReceive)> stageSettings)
    {
        Check.NotNull(campaign, nameof(Campaign));
        EnsurePromotionModule(campaign, PromotionModule.ShoppingCredit);

        var shoppingCredit = new CampaignShoppingCredit(GuidGenerator.Create(), campaign.Id, canExpire,
            validForDays, calculationMethod, calculationPercentage, applicableItem, budget);

        foreach (var (spend, pointsToReceive) in stageSettings)
        {
            shoppingCredit.AddStageSetting(GuidGenerator.Create(), spend, pointsToReceive);
        }
        
        campaign.ShoppingCredit = shoppingCredit;
        return shoppingCredit;
    }

    public CampaignAddOnProduct AddAddOnProduct(Campaign campaign, Guid productId, int productAmount, int limitPerOrder,
        bool isUnlimitedQuantity, int? availableQuantity, AddOnDisplayPrice displayPrice, AddOnProductCondition productCondition,
        int? threshold)
    {
        Check.NotNull(campaign, nameof(Campaign));
        EnsurePromotionModule(campaign, PromotionModule.AddOnProduct);

        campaign.AddOnProduct = new CampaignAddOnProduct(GuidGenerator.Create(), campaign.Id, productId,
            productAmount, limitPerOrder, isUnlimitedQuantity, availableQuantity, displayPrice,
            productCondition, threshold);

        return campaign.AddOnProduct;
    }

    private void EnsurePromotionModule(Campaign campaign, PromotionModule expected)
    {
        if (campaign.PromotionModule != expected)
            throw new BusinessException($"Campaign module must be {expected}");
    }

}
