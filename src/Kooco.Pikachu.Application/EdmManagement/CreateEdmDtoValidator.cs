﻿using FluentValidation;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Extensions;
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

        RuleFor(x => x.ApplyToAllMembers)
            .NotNull()
            .WithMessage(Required(nameof(CreateEdmDto.MemberTags)));

        RuleFor(x => x.MemberTags)
            .NotEmpty()
            .When(x => x.ApplyToAllMembers == false)
            .WithMessage(Required(nameof(CreateEdmDto.MemberTags)));

        RuleFor(x => x.GroupBuyId)
            .NotNull()
            .WithMessage(Required("GroupBuy"));

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
            .NotEmpty()
            .WithMessage(Required(nameof(CreateEdmDto.Subject)));

        RuleFor(x => x.Subject)
            .MaximumLength(EdmConsts.MaxSubjectLength);

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage(Required(nameof(CreateEdmDto.Message)))
            .NotEqual(StringExtensions.DefaultQuillHtml)
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
