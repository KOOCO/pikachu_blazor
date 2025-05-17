using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Campaigns;

public class CampaignBuilder
{
    private readonly CreateCampaignDto _input;

    public CampaignBuilder(CreateCampaignDto input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }

    public async Task<Campaign> CreateAsync(CampaignManager manager)
    {
        var campaign = await manager.CreateAsync(
            _input.Name,
            Require(_input.StartDate, nameof(_input.StartDate)),
            Require(_input.EndDate, nameof(_input.EndDate)),
            _input.Description,
            _input.TargetAudience,
            Require(_input.PromotionModule, nameof(_input.PromotionModule)),
            Require(_input.ApplyToAllGroupBuys, nameof(_input.ApplyToAllGroupBuys)),
            _input.GroupBuyIds,
            _input.ApplyToAllProducts,
            _input.ProductIds
        );

        HandleModules(campaign, manager);

        return campaign;
    }

    public async Task<Campaign> UpdateAsync(Campaign campaign, CampaignManager manager)
    {
        await manager.UpdateAsync(
            campaign,
            _input.Name,
            Require(_input.StartDate, nameof(_input.StartDate)),
            Require(_input.EndDate, nameof(_input.EndDate)),
            _input.Description,
            _input.TargetAudience,
            Require(_input.PromotionModule, nameof(_input.PromotionModule)),
            Require(_input.ApplyToAllGroupBuys, nameof(_input.ApplyToAllGroupBuys)),
            _input.GroupBuyIds,
            _input.ApplyToAllProducts,
            _input.ProductIds
        );

        HandleModules(campaign, manager);

        return campaign;
    }

    private void HandleModules(Campaign campaign, CampaignManager manager)
    {
        switch (_input.PromotionModule)
        {
            case PromotionModule.Discount:
                AddDiscount(campaign, _input.Discount, manager);
                break;
            case PromotionModule.ShoppingCredit:
                AddShoppingCredit(campaign, _input.ShoppingCredit, manager);
                break;
            case PromotionModule.AddOnProduct:
                AddAddOnProduct(campaign, _input.AddOnProduct, manager);
                break;
        }
    }

    private static void AddDiscount(Campaign campaign, CreateCampaignDiscountDto discount, CampaignManager manager)
    {
        manager.AddCampaignDiscount(
            campaign,
            Require(discount.IsDiscountCodeRequired, nameof(discount.IsDiscountCodeRequired)),
            discount.DiscountCode,
            Require(discount.AvailableQuantity, nameof(discount.AvailableQuantity)),
            Require(discount.MaximumUsePerPerson, nameof(discount.MaximumUsePerPerson)),
            Require(discount.DiscountMethod, nameof(discount.DiscountMethod)),
            discount.MinimumSpendAmount,
            discount.ApplyToAllShippingMethods,
            discount.DeliveryMethods,
            Require(discount.DiscountType, nameof(discount.DiscountType)),
            discount.DiscountAmount,
            discount.DiscountPercentage,
            discount.CapAmount
        );
    }

    private static void AddShoppingCredit(Campaign campaign, CreateCampaignShoppingCreditDto credit, CampaignManager manager)
    {
        var stages = credit.CalculationMethod == CalculationMethod.StagedCalculation
            ? credit.StageSettings.Select(x => (Require(x.Spend, "Stage Spend"), Require(x.PointsToReceive, "Points"))).ToList()
            : [];

        manager.AddShoppingCredit(
            campaign,
            Require(credit.CanExpire, nameof(credit.CanExpire)),
            credit.ValidForDays,
            Require(credit.CalculationMethod, nameof(credit.CalculationMethod)),
            credit.CalculationPercentage,
            credit.CapAmount,
            credit.ApplicableToAddOnProducts,
            credit.ApplicableToShippingFees,
            Require(credit.Budget, nameof(credit.Budget)),
            stages
        );
    }

    private static void AddAddOnProduct(Campaign campaign, CreateCampaignAddOnProductDto dto, CampaignManager manager)
    {
        manager.AddAddOnProduct(
            campaign,
            Require(dto.ProductId, nameof(dto.ProductId)),
            Require(dto.ProductAmount, nameof(dto.ProductAmount)),
            Require(dto.LimitPerOrder, nameof(dto.LimitPerOrder)),
            Require(dto.IsUnlimitedQuantity, nameof(dto.IsUnlimitedQuantity)),
            dto.AvailableQuantity,
            Require(dto.DisplayPrice, nameof(dto.DisplayPrice)),
            Require(dto.ProductCondition, nameof(dto.ProductCondition)),
            dto.Threshold
        );
    }

    private static T Require<T>(T? value, string name) where T : struct =>
        value ?? throw new UserFriendlyException($"{name} is required");
}