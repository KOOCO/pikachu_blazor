using Hangfire;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Members;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.EdmManagement;

public class EdmEmailService : ITransientDependency
{
    private readonly IEdmRepository _edmRepository;
    private readonly IEmailSender _emailSender;
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ILogger<EdmEmailService> _logger;

    public EdmEmailService(
        IEdmRepository edmRepository,
        IEmailSender emailSender,
        IMemberRepository memberRepository,
        ICurrentTenant currentTenant,
        ICampaignRepository campaignRepository,
        ILogger<EdmEmailService> logger
        )
    {
        _edmRepository = edmRepository;
        _emailSender = emailSender;
        _memberRepository = memberRepository;
        _currentTenant = currentTenant;
        _campaignRepository = campaignRepository;
        _logger = logger;
    }

    public Task EnqueueJob(Edm edm)
    {
        string? jobId = null;

        if (edm.TemplateType == EdmTemplateType.ShoppingCart)
        {
            var hour = edm.SendTimeUtc.Hour;
            var minute = edm.SendTimeUtc.Minute;

            string cron = edm.SendFrequency switch
            {
                EdmSendFrequency.Day => $"{minute} {hour} * * *",
                EdmSendFrequency.Week => $"{minute} {hour} * * 1",
                EdmSendFrequency.Month => $"{minute} {hour} 1 * *",
                _ => $"{minute} {hour} * * *"
            };

            jobId = edm.Id.ToString();

            if (!string.IsNullOrWhiteSpace(edm.JobId))
            {
                RecurringJob.RemoveIfExists(edm.JobId);
            }

            RecurringJob.AddOrUpdate<EdmEmailingJob>(
                jobId,
                job => job.ExecuteAsync(edm.Id),
                cron,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
            );
        }
        else
        {
            var utcStartDate = edm.StartDate.ToUniversalTime();

            var scheduledUtc = DateTime.SpecifyKind(
                utcStartDate.Date.Add(edm.SendTimeUtc.TimeOfDay),
                DateTimeKind.Utc
            );

            if (!string.IsNullOrWhiteSpace(edm.JobId))
            {
                BackgroundJob.Delete(edm.JobId);
            }

            jobId = BackgroundJob.Schedule<EdmEmailingJob>(
                sender => sender.ExecuteAsync(edm.Id),
                scheduledUtc
            );
        }

        edm.SetJobId(jobId);
        return Task.CompletedTask;
    }

    public async Task SendEmailAsync(Edm edm)
    {
        try
        {
            using (_currentTenant.Change(edm.TenantId))
            {
                var members = await _memberRepository.GetEdmMemberNameAndEmailAsync(edm.ApplyToAllMembers, edm.MemberTags);

                if (members.Count == 0)
                {
                    return;
                }

                var subject = edm.Subject;

                var template = await GetTemplate(edm);

                foreach (var (name, email) in members)
                {
                    template = template.Replace("{{memberName}}", name);

                    await _emailSender.SendAsync(
                        email,
                        edm.Subject,
                        template
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
            throw;
        }
    }

    private async Task<string> GetTemplate(Edm edm)
    {
        await _edmRepository.EnsurePropertyLoadedAsync(edm, e => e.GroupBuy);

        var templateName = edm.TemplateType switch
        {
            EdmTemplateType.Customize => "edm_campaign_main",
            EdmTemplateType.Campaign => "edm_campaign_main",
            EdmTemplateType.ShoppingCart => "edm_campaign_main",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(templateName))
            return string.Empty;

        var templatePath = Path.Combine("wwwroot", "EmailTemplates", "Edms", $"{templateName}.html");
        var template = File.ReadAllText(templatePath)
            .Replace("{{groupBuyName}}", edm.GroupBuy?.GroupBuyName ?? "N/A")
            .Replace("{{message}}", edm.Message);

        if (edm.TemplateType == EdmTemplateType.Campaign && edm.CampaignId.HasValue)
        {
            var campaign = await _campaignRepository.GetWithDetailsAsync(edm.CampaignId.Value);

            template = template
                .Replace("{{campaignName}}", campaign.Name)
                .Replace("{{campaignPeriod}}", $"{campaign.StartDate:dd/MM/yyyy} to {campaign.EndDate:dd/MM/yyyy}");

            template = InjectCampaignProperties(campaign, template);
        }

        return template;
    }

    private string InjectCampaignProperties(Campaign campaign, string template)
    {
        var propTemplatePath = Path.Combine("wwwroot", "EmailTemplates", "Edms", "edm_campaign_property_cell.html");
        var propTemplate = File.ReadAllText(propTemplatePath);

        var properties = campaign.PromotionModule switch
        {
            PromotionModule.Discount when campaign.Discount != null => GetDiscountProperties(campaign.Discount),
            PromotionModule.ShoppingCredit when campaign.ShoppingCredit != null => GetShoppingCreditProperties(campaign.ShoppingCredit),
            PromotionModule.AddOnProduct when campaign.AddOnProduct != null => GetAddOnProductProperties(campaign.AddOnProduct),
            _ => Enumerable.Empty<CampaignProperty>()
        };

        var propsHtml = string.Join("", properties.Select(p =>
            propTemplate.Replace("{{PropIcon}}", p.Icon)
                        .Replace("{{PropName}}", p.Name)
                        .Replace("{{PropValue}}", p.Value)));

        return template.Replace("{{edm_campaign_properties}}", propsHtml);
    }

    private List<CampaignProperty> GetDiscountProperties(CampaignDiscount discount)
    {
        return
        [
            new("💸", "Discount", discount.DiscountCode ?? "N/A"),
            new("🏷", "YourCode", discount.DiscountType == DiscountType.FixedAmount
                ? $"${discount.DiscountAmount}"
                : $"{discount.DiscountPercentage}%"),
            new("💵", "MinimumSpend", discount.MinimumSpendAmount?.ToString() ?? "N/A"),
            new("👤", "MaxUsePerPerson", discount.MaximumUsePerPerson.ToString())
        ];
    }

    private List<CampaignProperty> GetShoppingCreditProperties(CampaignShoppingCredit credit)
    {
        return
        [
            new("🏷", "Discount", credit.CalculationMethod == CalculationMethod.UnifiedCalculation
                ? $"{credit.CalculationPercentage}%"
                : $"${credit.GetMaxPointsToReceive()}"),
            new("💵", "UsagePeriod", credit.ValidForDays?.ToString() ?? "N/A")
        ];
    }

    private List<CampaignProperty> GetAddOnProductProperties(CampaignAddOnProduct addOn)
    {
        return
        [
            new("🏷", "Discount", addOn.ProductAmount.ToString()),
            new("💵", "MinimumSpend", addOn.Threshold?.ToString() ?? "N/A"),
            new("💵", "LimitPerOrder", addOn.LimitPerOrder.ToString())
        ];
    }

    private record CampaignProperty(string Icon, string Name, string Value);
}
