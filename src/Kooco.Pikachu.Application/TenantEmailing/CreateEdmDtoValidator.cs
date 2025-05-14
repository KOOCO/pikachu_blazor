using FluentValidation;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;

namespace Kooco.Pikachu.TenantEmailing;

public class CreateEdmDtoValidator : AbstractValidator<CreateEdmDto>
{
    public CreateEdmDtoValidator(IStringLocalizer<PikachuResource> l)
    {
        RuleFor(x => x.TemplateType)
            .NotNull();

        RuleFor(x => x.CampaignId)
            .NotNull()
            .When(x => x.TemplateType == EdmTemplateType.Campaign)
            .WithMessage(Required(nameof(Campaign)));

        RuleFor(x => x.MemberTags)
            .NotEmpty()
            .WithMessage(Required(nameof(CreateEdmDto.MemberTags)));

        RuleFor(x => x.ApplyToAllGroupBuys)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.ApplyToAllGroupBuys)));

        RuleFor(x => x.GroupBuyId)
            .NotNull()
            .When(x => x.ApplyToAllGroupBuys == false)
            .WithMessage(Required(nameof(CreateEdmDto.GroupBuyId)));

        When(x => x.TemplateType == EdmTemplateType.ShoppingCart, () =>
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(Required("SendDateRange"));

            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage(Required("SendDateRange"));
        }).Otherwise(() =>
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(Required("SendDate"));
        });

        RuleFor(x => x.SendTime)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.SendTime)));

        RuleFor(x => x.SendFrequency)
            .NotNull()
            .When(x => x.TemplateType == EdmTemplateType.ShoppingCart)
            .WithMessage(Required(nameof(CreateEdmDto.SendFrequency)));

        RuleFor(x => x.Subject)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.Subject)));

        RuleFor(x => x.Message)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.Message)))
            .NotEqual("<p><br></p>")
            .WithMessage(Required(nameof(CreateEdmDto.Message)));

        string Required(string propName)
        {
            return l["TheFieldIsRequired", l[propName]];
        }
    }
}
