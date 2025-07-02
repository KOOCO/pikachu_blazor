using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Campaigns;

public class CampaignManager : DomainService
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IRepository<CampaignDiscount, Guid> _discountRepository;
    private readonly IRepository<CampaignShoppingCredit, Guid> _shoppingCreditRepository;
    private readonly IRepository<CampaignAddOnProduct, Guid> _addOnProductRepository;

    public CampaignManager(
        ICampaignRepository campaignRepository,
        IRepository<CampaignDiscount, Guid> discountRepository,
        IRepository<CampaignShoppingCredit, Guid> shoppingCreditRepository,
        IRepository<CampaignAddOnProduct, Guid> addOnProductRepository)
    {
        _campaignRepository = campaignRepository;
        _discountRepository = discountRepository;
        _shoppingCreditRepository = shoppingCreditRepository;
        _addOnProductRepository = addOnProductRepository;
    }

    public async Task<Campaign> CreateAsync(
        string name,
        DateTime startDate,
        DateTime endDate,
        string? description,
        IEnumerable<string> targetAudience,
        PromotionModule promotionModule,
        bool applyToAllGroupBuys,
        IEnumerable<Guid> groupBuyIds,
        bool? applyToAllProducts,
        IEnumerable<Guid> productIds,
        CampaignUsagePolicy discountUsagePolicy,
        IEnumerable<Guid> allowedDiscountIds,
        CampaignUsagePolicy shoppingCreditUsagePolicy,
        IEnumerable<Guid> allowedShoppingCreditIds,
        CampaignUsagePolicy addOnProductUsagePolicy,
        IEnumerable<Guid> allowedAddOnProductIds,
        bool autoSave = true
        )
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

        var existing = await _campaignRepository.FirstOrDefaultAsync(c => c.Name == name);
        if (existing != null)
        {
            throw new CampaignWithSameNameAlreadyExistsException(name);
        }

        var campaign = new Campaign(
            GuidGenerator.Create(),
            name,
            startDate,
            endDate,
            description,
            targetAudience,
            promotionModule,
            applyToAllGroupBuys,
            applyToAllProducts
            );

        if (autoSave)
        {
            await _campaignRepository.InsertAsync(campaign);
        }

        AddGroupBuys(campaign, groupBuyIds, false);
        AddProducts(campaign, productIds, false);
        AddAllowedDiscounts(campaign, discountUsagePolicy, allowedDiscountIds);
        AddAllowedShoppingCredits(campaign, shoppingCreditUsagePolicy, allowedShoppingCreditIds);
        AddAllowedAddOnProducts(campaign, addOnProductUsagePolicy, allowedAddOnProductIds);

        return campaign;
    }

    public async Task<Campaign> UpdateAsync(
        Campaign campaign,
        string name,
        DateTime startDate,
        DateTime endDate,
        string? description,
        IEnumerable<string> targetAudience,
        PromotionModule promotionModule,
        bool applyToAllGroupBuys,
        IEnumerable<Guid> groupBuyIds,
        bool? applyToAllProducts,
        IEnumerable<Guid> productIds,
        CampaignUsagePolicy discountUsagePolicy,
        IEnumerable<Guid> allowedDiscountIds,
        CampaignUsagePolicy shoppingCreditUsagePolicy,
        IEnumerable<Guid> allowedShoppingCreditIds,
        CampaignUsagePolicy addOnProductUsagePolicy,
        IEnumerable<Guid> allowedAddOnProductIds
        )
    {
        Check.NotNull(campaign, nameof(campaign));
        
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

        var existing = await _campaignRepository.FirstOrDefaultAsync(c => c.Name == name);
        if (existing != null && existing.Id != campaign.Id)
        {
            throw new CampaignWithSameNameAlreadyExistsException(name);
        }

        var oldPromotionModule = campaign.PromotionModule;

        campaign.SetName(name);
        campaign.SetDateRange(startDate, endDate);
        campaign.SetDescription(description);
        campaign.SetTargetAudience(targetAudience);
        campaign.PromotionModule = promotionModule;
        campaign.ApplyToAllGroupBuys = applyToAllGroupBuys;
        campaign.SetApplyToAllProducts(applyToAllProducts);

        campaign.GroupBuys.RemoveAll(gb => !groupBuyIds.Contains(gb.GroupBuyId));
        campaign.Products.RemoveAll(p => !productIds.Contains(p.ProductId));

        AddGroupBuys(campaign, groupBuyIds, false);
        AddProducts(campaign, productIds, false);
        AddAllowedDiscounts(campaign, discountUsagePolicy, allowedDiscountIds);
        AddAllowedShoppingCredits(campaign, shoppingCreditUsagePolicy, allowedShoppingCreditIds);
        AddAllowedAddOnProducts(campaign, addOnProductUsagePolicy, allowedAddOnProductIds);

        if (campaign.PromotionModule != oldPromotionModule)
        {
            await RemoveModulesAsync(campaign);
        }

        await _campaignRepository.UpdateAsync(campaign);
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
        else
        {
            campaign.GroupBuys.Clear();
        }
    }

    public CampaignGroupBuy AddGroupBuy(Campaign campaign, Guid groupBuyId)
    {
        Check.NotNull(campaign, nameof(Campaign));
        Check.NotDefaultOrNull<Guid>(groupBuyId, nameof(groupBuyId));

        if (!campaign.GroupBuys.Any(gb => gb.GroupBuyId == groupBuyId))
        {
            return campaign.AddGroupBuy(GuidGenerator.Create(), groupBuyId);
        }
        else
        {
            return campaign.GroupBuys.First(gb => gb.GroupBuyId == groupBuyId);
        }
    }

    public void AddProducts(Campaign campaign, IEnumerable<Guid> productIds, bool throwException = true)
    {
        var canAdd = campaign.ApplyToAllProducts == false;
        if (throwException && !canAdd)
        {
            throw new BusinessException("Products can not be added to this Campaign. Apply to All Products is checked.");
        }

        if (canAdd)
        {
            foreach (var productId in productIds)
            {
                AddProduct(campaign, productId);
            }
        }
        else
        {
            campaign.Products.Clear();
        }
    }

    public CampaignProduct AddProduct(Campaign campaign, Guid productId)
    {
        Check.NotNull(campaign, nameof(Campaign));
        Check.NotDefaultOrNull<Guid>(productId, nameof(productId));

        if (!campaign.Products.Any(p => p.ProductId == productId))
        {
            return campaign.AddProduct(GuidGenerator.Create(), productId);
        }
        else
        {
            return campaign.Products.First(p => p.ProductId == productId);
        }
    }

    public void AddAllowedDiscounts(
        Campaign campaign,
        CampaignUsagePolicy discountUsagePolicy,
        IEnumerable<Guid> allowedDiscountIds
        )
    {
        if (discountUsagePolicy == CampaignUsagePolicy.Specific && !allowedDiscountIds.Any())
        {
            throw new BusinessException("Required", nameof(allowedDiscountIds));
        }

        campaign.DiscountUsagePolicy = discountUsagePolicy;
        
        AddUseableCampaigns(campaign, allowedDiscountIds, PromotionModule.Discount, discountUsagePolicy);
    }

    public void AddAllowedShoppingCredits(
        Campaign campaign,
        CampaignUsagePolicy shoppingCreditUsagePolicy,
        IEnumerable<Guid> allowedShoppingCreditIds
        )
    {
        if (shoppingCreditUsagePolicy == CampaignUsagePolicy.Specific && !allowedShoppingCreditIds.Any())
        {
            throw new BusinessException("Required", nameof(allowedShoppingCreditIds));
        }

        campaign.ShoppingCreditUsagePolicy = shoppingCreditUsagePolicy;
        
        AddUseableCampaigns(campaign, allowedShoppingCreditIds, PromotionModule.ShoppingCredit, shoppingCreditUsagePolicy);
    }

    public void AddAllowedAddOnProducts(
        Campaign campaign,
        CampaignUsagePolicy addOnUsagePolicy,
        IEnumerable<Guid> allowedAddOnProductIds
        )
    {
        if (addOnUsagePolicy == CampaignUsagePolicy.Specific && !allowedAddOnProductIds.Any())
        {
            throw new BusinessException("Required", nameof(allowedAddOnProductIds));
        }

        campaign.AddOnProductUsagePolicy = addOnUsagePolicy;

        AddUseableCampaigns(campaign, allowedAddOnProductIds, PromotionModule.AddOnProduct, addOnUsagePolicy);
    }

    public void AddUseableCampaigns(
        Campaign campaign,
        IEnumerable<Guid> allowedIds,
        PromotionModule promotionModule,
        CampaignUsagePolicy usagePolicy
        )
    {
        Check.NotNull(campaign, nameof(Campaign));

        if (usagePolicy != CampaignUsagePolicy.Specific)
        {
            campaign.UseableCampaigns.RemoveAll(uc => uc.PromotionModule == promotionModule);
        }
        else
        {
            campaign.UseableCampaigns.RemoveAll(uc =>
                uc.PromotionModule == promotionModule
                && !allowedIds.Contains(uc.AllowedCampaignId)
                );

            foreach (var allowedId in allowedIds)
            {
                Check.NotDefaultOrNull<Guid>(allowedId, nameof(allowedId));

                campaign.UseableCampaigns.RemoveAll(uc =>
                    uc.PromotionModule != promotionModule
                    && uc.AllowedCampaignId == allowedId
                    );

                var alreadyAdded = campaign.UseableCampaigns
                    .FirstOrDefault(uc =>
                    uc.PromotionModule == promotionModule
                    && uc.AllowedCampaignId == allowedId
                    );

                if (alreadyAdded == null)
                {
                    campaign.AddUseableCampaign(
                        GuidGenerator.Create(),
                        allowedId,
                        promotionModule
                        );
                }
            }
        }
    }

    public CampaignDiscount AddCampaignDiscount(Campaign campaign, bool isDiscountCodeRequired, string? discountCode, int availableQuantity,
        int maximumUsePerPerson, DiscountMethod discountMethod, int? minimumSpendAmount, bool? applyToAllShippingMethods,
        IEnumerable<DeliveryMethod> deliveryMethods, DiscountType discountType, int? discountAmount, int? discountPercentage, double? capAmount)
    {
        Check.NotNull(campaign, nameof(Campaign));
        EnsurePromotionModule(campaign, PromotionModule.Discount);

        campaign.Discount = new CampaignDiscount(GuidGenerator.Create(), campaign.Id, isDiscountCodeRequired, discountCode,
            availableQuantity, maximumUsePerPerson, discountMethod, minimumSpendAmount, applyToAllShippingMethods,
            deliveryMethods, discountType, discountAmount, discountPercentage, capAmount);

        return campaign.Discount;
    }

    public CampaignShoppingCredit AddShoppingCredit(
        Campaign campaign, 
        bool canExpire, 
        int? validForDays,
        CalculationMethod calculationMethod, 
        double? calculationPercentage, 
        double? capAmount, 
        bool applicableToAddOnProducts,
        bool applicableToShippingFees, 
        int budget,
        CampaignSpendCondition spendCondition,
        int? threshold,
        List<(int spend, int pointsToReceive)> stageSettings
        )
    {
        Check.NotNull(campaign, nameof(Campaign));
        EnsurePromotionModule(campaign, PromotionModule.ShoppingCredit);

        int maxPointsToReceive = stageSettings.OrderByDescending(ss => ss.pointsToReceive).Select(ss => ss.pointsToReceive).FirstOrDefault();

        var shoppingCredit = new CampaignShoppingCredit(
            GuidGenerator.Create(), 
            campaign.Id, 
            canExpire,
            validForDays, 
            calculationMethod, 
            calculationPercentage, 
            capAmount, 
            applicableToAddOnProducts,
            applicableToShippingFees, 
            budget, 
            maxPointsToReceive,
            spendCondition,
            threshold
            );

        if (calculationMethod == CalculationMethod.StagedCalculation)
        {
            foreach (var (spend, pointsToReceive) in stageSettings)
            {
                shoppingCredit.AddStageSetting(GuidGenerator.Create(), spend, pointsToReceive);
            }
        }

        campaign.ShoppingCredit = shoppingCredit;
        return shoppingCredit;
    }

    public CampaignAddOnProduct AddAddOnProduct(
        Campaign campaign, 
        Guid productId, 
        Guid itemDetailId, 
        int productAmount, 
        int limitPerOrder,
        bool isUnlimitedQuantity, 
        int? availableQuantity, 
        AddOnDisplayPrice displayPrice, 
        CampaignSpendCondition spendCondition,
        int? threshold
        )
    {
        Check.NotNull(campaign, nameof(Campaign));
        EnsurePromotionModule(campaign, PromotionModule.AddOnProduct);

        campaign.AddOnProduct = new CampaignAddOnProduct(
            GuidGenerator.Create(), 
            campaign.Id, 
            productId,
            itemDetailId, 
            productAmount, 
            limitPerOrder, 
            isUnlimitedQuantity, 
            availableQuantity, 
            displayPrice,
            spendCondition, 
            threshold
            );

        return campaign.AddOnProduct;
    }

    private static void EnsurePromotionModule(Campaign campaign, PromotionModule expected)
    {
        if (campaign.PromotionModule != expected)
            throw new BusinessException($"Campaign module must be {expected}");
    }

    public async Task RemoveDiscountAsync(Campaign campaign)
    {
        if (campaign.Discount != null)
        {
            await _discountRepository.DeleteAsync(campaign.Discount);
            campaign.Discount = null!;
        }
    }

    public async Task RemoveShoppingCreditAsync(Campaign campaign)
    {
        if (campaign.ShoppingCredit != null)
        {
            await _shoppingCreditRepository.DeleteAsync(campaign.ShoppingCredit);
            campaign.ShoppingCredit = null!;
        }
    }

    public async Task RemoveAddOnProductAsync(Campaign campaign)
    {
        if (campaign.AddOnProduct != null)
        {
            await _addOnProductRepository.DeleteAsync(campaign.AddOnProduct);
            campaign.AddOnProduct = null!;
        }
    }

    public async Task RemoveModulesAsync(Campaign campaign)
    {
        await RemoveDiscountAsync(campaign);
        await RemoveShoppingCreditAsync(campaign);
        await RemoveAddOnProductAsync(campaign);
    }
}
