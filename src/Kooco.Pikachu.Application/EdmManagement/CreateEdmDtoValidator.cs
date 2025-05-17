using FluentValidation;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using System;

namespace Kooco.Pikachu.EdmManagement;

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

        RuleFor(x => x.MemberType)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.MemberTags)));

        RuleFor(x => x.MemberTags)
            .NotEmpty()
            .When(x => x.MemberType == EdmMemberType.SpecificMemberTags)
            .WithMessage(Required(nameof(CreateEdmDto.MemberTags)));

        RuleFor(x => x.ApplyToAllGroupBuys)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.ApplyToAllGroupBuys)));

        RuleFor(x => x.GroupBuyIds)
            .NotEmpty()
            .When(x => x.ApplyToAllGroupBuys == false)
            .WithMessage(Required("GroupBuys"));

        RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(x => Required(x.TemplateType == EdmTemplateType.ShoppingCart ? "SendDateRange" : "SendDate"));

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithName(l["SendDateRange"])
            .WithMessage(x => GreaterOrEqualTo(x.TemplateType == EdmTemplateType.ShoppingCart ? "SendDateRange" : "SendDate", DateFormat(DateTime.Today)));

        When(x => x.TemplateType == EdmTemplateType.ShoppingCart, () =>
        {
            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage(Required("SendDateRange"));

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue)
                .WithMessage(x => GreaterOrEqualTo("SendDateRange", DateFormat(x.StartDate)));
        });

        RuleFor(x => x.SendTime)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.SendTime)));

        RuleFor(x => x.SendTime)
            .Must((x, sendTime) =>
            {
                if (!sendTime.HasValue || !x.StartDate.HasValue)
                    return true;

                if (x.StartDate.Value.Date != DateTime.Today)
                    return true;

                var now = DateTime.Now.AddMinutes(1);
                var nowTime = new TimeSpan(now.Hour, now.Minute, 0);
                var sendTimeOnly = new TimeSpan(sendTime.Value.Hour, sendTime.Value.Minute, 0);

                return sendTimeOnly >= nowTime;
            })
            .WithMessage(GreaterOrEqualTo(nameof(CreateEdmDto.SendTime), TimeFormat(DateTime.Now.AddMinutes(1))));

        RuleFor(x => x.SendFrequency)
            .NotNull()
            .When(x => x.TemplateType == EdmTemplateType.ShoppingCart)
            .WithMessage(Required(nameof(CreateEdmDto.SendFrequency)));

        RuleFor(x => x.Subject)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.Subject)));

        RuleFor(x => x.Subject)
            .MaximumLength(EdmConsts.MaxSubjectLength);

        RuleFor(x => x.Message)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.Message)))
            .NotEqual("<p><br></p>")
            .WithMessage(Required(nameof(CreateEdmDto.Message)));

        string Required(string propName)
        {
            return l["TheFieldIsRequired", l[propName]];
        }

        string GreaterOrEqualTo(string propName, object value)
        {
            return l["TheFieldMustBeGreaterOrEqualTo", l[propName], value];
        }

        string DateFormat(DateTime? date)
        {
            return date?.ToString("yyyy-MM-dd") ?? "";
        }

        string TimeFormat(DateTime? time)
        {
            return time?.ToShortTimeString() ?? "";
        }
    }
}
