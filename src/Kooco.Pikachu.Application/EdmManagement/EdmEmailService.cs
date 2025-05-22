using Hangfire;
using Kooco.Pikachu.Members;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.EdmManagement;

public class EdmEmailService : ITransientDependency
{
    private readonly IEdmRepository _edmRepository;
    private readonly IEmailSender _emailSender;
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<EdmEmailService> _logger;

    public EdmEmailService(
        IEdmRepository edmRepository,
        IEmailSender emailSender,
        IMemberRepository memberRepository,
        ICurrentTenant currentTenant,
        ILogger<EdmEmailService> logger
        )
    {
        _edmRepository = edmRepository;
        _emailSender = emailSender;
        _memberRepository = memberRepository;
        _currentTenant = currentTenant;
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
            var scheduledUtc = DateTime.SpecifyKind(
                edm.StartDate.Date.Add(edm.SendTimeUtc.TimeOfDay),
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
                var members = await _memberRepository.GetEdmMemberEmailsAsync(edm.ApplyToAllMembers, edm.MemberTags);

                if (members.Count == 0)
                {
                    return;
                }

                var groupBuyNames = new List<string>();// await _edmRepository.GetGroupBuyNamesAsync([.. edm.GroupBuys.Select(gb => gb.GroupBuyId)]);

                var emailTitle = edm.TemplateType switch
                {
                    EdmTemplateType.Customize => "This is CUSTOMIZE email template!",
                    EdmTemplateType.Campaign => "This is CAMPAIGN email template!",
                    EdmTemplateType.ShoppingCart => "This is SHOPPING CART email template!",
                    _ => ""
                };

                var template = @$"
                <table>
                    <tr>
                      <td>
                        <h3>{emailTitle}</h3>
                        <div>
                          {edm.Message}
                        </div>
                        {{groupbuys}}
                      </td>
                    </tr>
                </table>
            ";

                string groupBuyTemplate = "<h5>Group Buy Names:</h5>";
                foreach (var groupBuyName in groupBuyNames)
                {
                    groupBuyTemplate += $"<div>{groupBuyName}</div>";
                }

                template = template.Replace("{groupbuys}", groupBuyTemplate);
                foreach (var member in members)
                {
                    await _emailSender.SendAsync(
                        member,
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
}
