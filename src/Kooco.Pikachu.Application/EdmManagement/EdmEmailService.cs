using Hangfire;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Tenants;
using Microsoft.Extensions.Logging;
using System;
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
    private readonly ITenantSettingsAppService _tenantSettingsAppService;
    public EdmEmailService(
        IEdmRepository edmRepository,
        IEmailSender emailSender,
        IMemberRepository memberRepository,
        ICurrentTenant currentTenant,
        ICampaignRepository campaignRepository,
        ILogger<EdmEmailService> logger,
        ITenantSettingsAppService tenantSettingsAppService
        )
    {
        _edmRepository = edmRepository;
        _emailSender = emailSender;
        _memberRepository = memberRepository;
        _currentTenant = currentTenant;
        _campaignRepository = campaignRepository;
        _logger = logger;
        _tenantSettingsAppService = tenantSettingsAppService;
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

                await _edmRepository.EnsurePropertyLoadedAsync(edm, edm => edm.GroupBuy);

                var subject = edm.Subject;

                var tenantSettings = await _tenantSettingsAppService.FirstOrDefaultAsync();

                var template = await EdmTemplateBuilder.Build(edm, tenantSettings, _campaignRepository);

                foreach (var (name, email) in members)
                {
                    var memberTemplate = template.Replace("{{MemberName}}", name);

                    await _emailSender.SendAsync(
                        email,
                        edm.Subject,
                        memberTemplate
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
}
