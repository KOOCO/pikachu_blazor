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

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate);

        RuleFor(x => x.Name)
            .MaximumLength(CampaignConsts.MaxNameLength);

        RuleFor(x => x.Description)
            .MaximumLength(CampaignConsts.MaxDescriptionLength);

        AddRequiredRule(x => x.ApplyToAllGroupBuys);

        RuleFor(x => x.GroupBuyIds)
            .NotEmpty()
            .When(x => x.PromotionModule.HasValue && x.ApplyToAllGroupBuys != null && x.ApplyToAllGroupBuys == false)
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.GroupBuyIds)]]);

        When(x => x.PromotionModule.HasValue && x.PromotionModule != PromotionModule.AddOnProduct, () =>
        {
            AddRequiredRule(x => x.ApplyToAllProducts);

            RuleFor(x => x.ProductIds)
                .NotEmpty()
                .When(x => x.PromotionModule.HasValue && x.ApplyToAllProducts != null && x.ApplyToAllProducts == false)
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDto.ProductIds)]]);
        });

        When(x => x.PromotionModule == PromotionModule.Discount, () =>
        {
            RuleFor(x => x.Discount)
                .NotNull()
                .SetValidator(new CreateCampaignDiscountDtoValidator(l));
        });

        When(x => x.PromotionModule == PromotionModule.ShoppingCredit, () =>
        {
            RuleFor(x => x.ShoppingCredit)
                .NotNull()
                .SetValidator(new CreateCampaignShoppingCreditDtoValidator(l));
        });

        When(x => x.PromotionModule == PromotionModule.AddOnProduct, () =>
        {
            RuleFor(x => x.AddOnProduct)
                .NotNull()
                .SetValidator(new CreateCampaignAddOnProductDtoValidator(l));
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

public class CreateCampaignDiscountDtoValidator : AbstractValidator<CreateCampaignDiscountDto>
{
    public CreateCampaignDiscountDtoValidator(IStringLocalizer<PikachuResource> l)
    {
        RuleFor(d => d.IsDiscountCodeRequired)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.DiscountCode)]]);

        When(d => d.IsDiscountCodeRequired == true, () =>
        {
            RuleFor(d => d.DiscountCode)
                .NotEmpty()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.DiscountCode)]]);
        });

        RuleFor(d => d.AvailableQuantity)
            .NotEmpty()
            .WithMessage(l["TheFieldIsRequired", l["NoOfIssuedCodes"]]);

        RuleFor(d => d.AvailableQuantity)
            .GreaterThanOrEqualTo(0)
            .WithName(l["NoOfIssuedCodes"]);

        RuleFor(d => d.MaximumUsePerPerson)
            .NotEmpty()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.MaximumUsePerPerson)]]);

        RuleFor(d => d.MaximumUsePerPerson)
            .GreaterThanOrEqualTo(0)
            .WithName(l[nameof(CreateCampaignDiscountDto.MaximumUsePerPerson)]);

        RuleFor(d => d.DiscountMethod)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.DiscountMethod)]]);

        When(d => d.DiscountMethod == DiscountMethod.MinimumSpendAmount, () =>
        {
            RuleFor(d => d.MinimumSpendAmount)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.MinimumSpendAmount)]])
                .GreaterThanOrEqualTo(0);
        });

        When(d => d.DiscountMethod == DiscountMethod.ShippingDiscount, () =>
        {
            RuleFor(d => d.ApplyToAllShippingMethods)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.ApplyToAllShippingMethods)]]);

            When(d => d.ApplyToAllShippingMethods == false, () =>
            {
                RuleFor(d => d.DeliveryMethods)
                    .NotEmpty()
                    .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.DeliveryMethods)]]);
            });
        });

        RuleFor(d => d.DiscountType)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.DiscountType)]]);

        When(d => d.DiscountType == DiscountType.FixedAmount, () =>
        {
            RuleFor(d => d.DiscountAmount)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.DiscountAmount)]])
                .GreaterThanOrEqualTo(0);
        });

        When(d => d.DiscountType == DiscountType.Percentage, () =>
        {
            RuleFor(d => d.DiscountPercentage)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignDiscountDto.DiscountPercentage)]])
                .InclusiveBetween(0, 100);
        });
    }
}

public class CreateCampaignShoppingCreditDtoValidator : AbstractValidator<CreateCampaignShoppingCreditDto>
{
    public CreateCampaignShoppingCreditDtoValidator(IStringLocalizer<PikachuResource> l)
    {
        RuleFor(x => x.CanExpire)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l["UsagePeriod"]]);

        When(x => x.CanExpire == true, () =>
        {
            RuleFor(x => x.ValidForDays)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignShoppingCreditDto.ValidForDays)]]);

            RuleFor(x => x.ValidForDays)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ValidForDays != null);
        });

        RuleFor(x => x.CalculationMethod)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignShoppingCreditDto.CalculationMethod)]]);

        When(x => x.CalculationMethod == CalculationMethod.UnifiedCalculation, () =>
        {
            RuleFor(x => x.CalculationPercentage)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignShoppingCreditDto.CalculationPercentage)]]);

            RuleFor(x => x.CalculationPercentage)
                .GreaterThanOrEqualTo(0)
                .When(x => x.CalculationPercentage != null);
        });

        RuleFor(x => x.ApplicableItem)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignShoppingCreditDto.ApplicableItem)]]);

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Budget != null);

        RuleFor(x => x.Budget)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignShoppingCreditDto.Budget)]]);

        When(x => x.CalculationMethod == CalculationMethod.StagedCalculation, () =>
        {
            RuleFor(x => x.StageSettings)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignShoppingCreditDto.StageSettings)]]);

            RuleForEach(x => x.StageSettings)
                .SetValidator(new StageSettingsDtoValidator(l))
                .When(x => x.StageSettings != null);
        });
    }
}

public class StageSettingsDtoValidator : AbstractValidator<CreateCampaignStageSettingDto>
{
    public StageSettingsDtoValidator(IStringLocalizer<PikachuResource> l)
    {
        RuleFor(s => s.Spend)
            .NotEmpty()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignStageSettingDto.Spend)]])
            .GreaterThanOrEqualTo(0);

        RuleFor(s => s.PointsToReceive)
            .NotEmpty()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignStageSettingDto.PointsToReceive)]])
            .GreaterThanOrEqualTo(0);
    }
}

public class CreateCampaignAddOnProductDtoValidator : AbstractValidator<CreateCampaignAddOnProductDto>
{
    public CreateCampaignAddOnProductDtoValidator(IStringLocalizer<PikachuResource> l)
    {
        RuleFor(x => x.ProductId)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.ProductId)]]);

        RuleFor(x => x.ProductAmount)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.ProductAmount)]]);

        RuleFor(x => x.ProductAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ProductAmount != null)
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.ProductAmount)]]);

        RuleFor(x => x.LimitPerOrder)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.LimitPerOrder)]]);

        RuleFor(x => x.LimitPerOrder)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LimitPerOrder != null);

        RuleFor(x => x.IsUnlimitedQuantity)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.IsUnlimitedQuantity)]]);

        When(x => x.IsUnlimitedQuantity == false, () =>
        {
            RuleFor(x => x.AvailableQuantity)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.AvailableQuantity)]])
                .GreaterThanOrEqualTo(0)
                .WithName(l[nameof(CreateCampaignAddOnProductDto.AvailableQuantity)]);
        });

        RuleFor(x => x.DisplayPrice)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.DisplayPrice)]]);

        RuleFor(x => x.ProductCondition)
            .NotNull()
            .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.ProductCondition)]]);

        When(x => x.ProductCondition == AddOnProductCondition.MustMeetSpecifiedThreshold, () =>
        {
            RuleFor(x => x.Threshold)
                .NotNull()
                .WithMessage(l["TheFieldIsRequired", l[nameof(CreateCampaignAddOnProductDto.Threshold)]]);

            RuleFor(x => x.Threshold)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Threshold != null);
        });
    }
}
