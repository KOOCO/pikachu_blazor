using AngleSharp.Dom;
using FluentValidation;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Linq.Expressions;

namespace Kooco.Pikachu.Campaigns;

public class CreateCampaignDtoValidator : AbstractValidator<CreateCampaignDto>
{
    private readonly IStringLocalizer<PikachuResource> _l;
    public CreateCampaignDtoValidator(IStringLocalizer<PikachuResource> l)
    {
        _l = l;

        AddRequiredRule(x => x.Name);
        AddRequiredRule(x => x.StartDate);
        AddRequiredRule(x => x.EndDate);
        AddRequiredRule(x => x.TargetAudience);
        AddRequiredRule(x => x.PromotionModule);

        RuleFor(x => x.Name)
            .MaximumLength(CampaignConsts.MaxNameLength);

        RuleFor(x => x.Description)
            .MaximumLength(CampaignConsts.MaxDescriptionLength);

        When(x => x.PromotionModule == PromotionModule.Discount, () =>
        {
            AddRequiredRule(x => x.IsDiscountCodeRequired, nameof(CreateCampaignDto.DiscountCode));

            When(x => x.IsDiscountCodeRequired == true, () =>
            {
                RuleFor(x => x.DiscountCode)
                    .NotEmpty()
                    .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.DiscountCode)]]);
            });

            RuleFor(x => x.AvailableQuantity)
                .GreaterThanOrEqualTo(0)
                .WithName(l["NoOfIssuedCodes"]);

            RuleFor(x => x.MaximumUsePerPerson)
                .GreaterThanOrEqualTo(0)
                .WithName(l["MaximumUsePerPerson"]);

            AddRequiredRule(x => x.ApplyToAllGroupBuys);

            RuleFor(x => x.GroupBuyIds)
                .NotEmpty()
                .When(x => x.ApplyToAllGroupBuys == false)
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.GroupBuyIds)]]);

            AddRequiredRule(x => x.ApplyToAllProducts);

            RuleFor(x => x.ProductIds)
                .NotEmpty()
                .When(x => x.ApplyToAllProducts == false)
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.ProductIds)]]);

            AddRequiredRule(x => x.DiscountMethod);

            When(x => x.DiscountMethod == DiscountMethod.MinimumSpendAmount, () =>
            {
                RuleFor(x => x.MinimumSpendAmount)
                    .NotEmpty()
                    .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.MinimumSpendAmount)]])
                    .GreaterThanOrEqualTo(0);
            });

            When(x => x.DiscountMethod == DiscountMethod.ShippingDiscount, () =>
            {
                AddRequiredRule(x => x.ApplyToAllShippingMethods);

                RuleFor(x => x.DeliveryMethods)
                    .NotEmpty()
                    .When(x => x.ApplyToAllShippingMethods == false)
                    .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.DeliveryMethods)]]);
            });

            AddRequiredRule(x => x.DiscountType);

            When(x => x.DiscountType == DiscountType.FixedAmount, () =>
            {
                RuleFor(x => x.DiscountAmount)
                    .NotEmpty()
                    .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.DiscountAmount)]])
                    .GreaterThanOrEqualTo(0);
            });

            When(x => x.DiscountType == DiscountType.Percentage, () =>
            {
                RuleFor(x => x.DiscountPercentage)
                    .NotEmpty()
                    .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.DiscountPercentage)]])
                    .GreaterThanOrEqualTo(0)
                    .LessThanOrEqualTo(100);
            });
        });

        When(x => x.PromotionModule == PromotionModule.ShoppingCredit, () =>
        {
            AddRequiredRule(x => x.CanExpire);

            AddRequiredRule(x => x.ValidForDays);
            RuleFor(x => x.ValidForDays)
                .GreaterThanOrEqualTo(0)
                .When(x => x.CanExpire == true && x.ValidForDays != null);

            AddRequiredRule(x => x.CalculationMethod);

            AddRequiredRule(x => x.CalculationPercentage);
            RuleFor(x => x.CalculationPercentage)
                .GreaterThanOrEqualTo(0)
                .When(x => x.CalculationMethod == CalculationMethod.UnifiedCalculation && x.CalculationPercentage != null);

            When(x => x.CalculationMethod == CalculationMethod.StagedCalculation, () =>
            {
                AddRequiredRule(x => x.StageSettings);

                RuleForEach(x => x.StageSettings)
                    .ChildRules(stage =>
                    {
                        stage.RuleFor(s => s.Spend)
                            .NotEmpty()
                            .WithMessage(l["TheFieldIsRequired", l[nameof(StageSettingsDto.Spend)]])
                            .GreaterThanOrEqualTo(0);

                        stage.RuleFor(s => s.PointsToReceive)
                            .NotEmpty()
                            .WithMessage(l["TheFieldIsRequired", l[nameof(StageSettingsDto.PointsToReceive)]])
                            .GreaterThanOrEqualTo(0);
                    })
                    .When(x => x.StageSettings != null);
            });

            AddRequiredRule(x => x.ApplicableItem);

            AddRequiredRule(x => x.ApplyToAllGroupBuys);

            RuleFor(x => x.GroupBuyIds)
                .NotEmpty()
                .When(x => x.ApplyToAllGroupBuys == false)
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.GroupBuyIds)]]);

            AddRequiredRule(x => x.ApplyToAllProducts);

            RuleFor(x => x.ProductIds)
                .NotEmpty()
                .When(x => x.ApplyToAllProducts == false)
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.ProductIds)]]);

            AddRequiredRule(x => x.Budget);
            RuleFor(x => x.Budget).GreaterThanOrEqualTo(0);
        });

        When(x => x.PromotionModule == PromotionModule.AddOnProduct, () =>
        {
            AddRequiredRule(x => x.AddOnProductId);
            AddRequiredRule(x => x.AddOnProductAmount);
            RuleFor(x => x.AddOnProductAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.AddOnProductAmount != null);
            
            AddRequiredRule(x => x.AddOnLimitPerOrder);
            RuleFor(x => x.AddOnLimitPerOrder)
                .GreaterThanOrEqualTo(0)
                .When(x => x.AddOnLimitPerOrder != null);

            AddRequiredRule(x => x.IsUnlimitedQuantity, nameof(CreateCampaignDto.AvailableQuantity));
            When(x => x.IsUnlimitedQuantity == false, () =>
            {
                RuleFor(x => x.AvailableQuantity)
                    .GreaterThanOrEqualTo(0)
                    .WithName(l[nameof(CreateCampaignDto.AvailableQuantity)]);
            });

            AddRequiredRule(x => x.AddOnDisplayPrice);
            AddRequiredRule(x => x.AddOnProductCondition);
            When(x => x.AddOnProductCondition == AddOnProductCondition.MustMeetSpecifiedThreshold, () =>
            {
                AddRequiredRule(x => x.Threshold);
                RuleFor(x => x.Threshold)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.Threshold != null);
            });

            AddRequiredRule(x => x.ApplyToAllGroupBuys);
            RuleFor(x => x.GroupBuyIds)
                .NotEmpty()
                .When(x => x.ApplyToAllGroupBuys == false)
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.GroupBuyIds)]]);
        });
    }

    private void AddRequiredRule<T>(
        Expression<Func<CreateCampaignDto, T>> expression,
        string? fieldNameOverride = null)
    {
        var displayName = _l[fieldNameOverride ?? GetPropertyName(expression)];

        var rule = RuleFor(expression);

        if (typeof(T) == typeof(bool?))
        {
            rule.NotNull()
                .WithMessage(_ => _l["TheFieldIsRequired", displayName]);
        }
        else
        {
            rule.NotEmpty()
                .WithMessage(_ => _l["TheFieldIsRequired", displayName]);
        }
    }

    private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
    {
        if (propertyLambda.Body is not MemberExpression member)
            throw new ArgumentException("Expression must be a member expression");
        return member.Member.Name;
    }
}

public class StageSettingsDtoValidator : AbstractValidator<StageSettingsDto>
{
    public StageSettingsDtoValidator(IStringLocalizer<PikachuResource> l)
    {
        RuleFor(x => x.Spend)
            .NotEmpty()
            .WithMessage(l["TheFieldIsRequired", l[nameof(StageSettingsDto.Spend)]])
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PointsToReceive)
            .NotEmpty()
            .WithMessage(l["TheFieldIsRequired", l[nameof(StageSettingsDto.PointsToReceive)]])
            .GreaterThanOrEqualTo(0);
    }
}
