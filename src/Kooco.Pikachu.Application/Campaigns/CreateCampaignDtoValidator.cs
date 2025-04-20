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
