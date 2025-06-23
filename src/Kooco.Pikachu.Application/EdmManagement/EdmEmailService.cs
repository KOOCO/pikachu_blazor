using Hangfire;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.ShopCarts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.EdmManagement;

public class EdmEmailService : ITransientDependency
{
    private readonly IEmailSender _emailSender;
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<EdmEmailService> _logger;
    private readonly IShopCartRepository _shopCartRepository;
    private readonly IConfiguration _configuration;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly EdmTemplateBuilder _edmTemplateBuilder;

    public EdmEmailService(
        IEmailSender emailSender,
        IMemberRepository memberRepository,
        ICurrentTenant currentTenant,
        ILogger<EdmEmailService> logger,
        IShopCartRepository shopCartRepository,
        IConfiguration configuration,
        IBackgroundJobManager backgroundJobManager,
        EdmTemplateBuilder edmTemplateBuilder
        )
    {
        _emailSender = emailSender;
        _memberRepository = memberRepository;
        _currentTenant = currentTenant;
        _logger = logger;
        _shopCartRepository = shopCartRepository;
        _configuration = configuration;
        _backgroundJobManager = backgroundJobManager;
        _edmTemplateBuilder = edmTemplateBuilder;
    }

    public async Task EnqueueJob(Edm edm)
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

            CancelJob(edm);

            RecurringJob.AddOrUpdate<EdmEmailingJob>(
                jobId,
                job => job.ExecuteAsync(new EdmEmailingJobArgs { EdmId = edm.Id }),
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

            CancelJob(edm);

            jobId = await _backgroundJobManager.EnqueueAsync(
                new EdmEmailingJobArgs { EdmId = edm.Id },
                delay: scheduledUtc - DateTime.UtcNow
            );
        }

        edm.SetJobId(jobId);
    }

    public void CancelJob(Edm edm)
    {
        if (!string.IsNullOrWhiteSpace(edm.JobId))
        {
            if (edm.TemplateType == EdmTemplateType.ShoppingCart)
            {
                RecurringJob.RemoveIfExists(edm.JobId);
            }
            else
            {
                BackgroundJob.Delete(edm.JobId);
            }
            edm.SetJobId(null);
        }
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

                List<ShopCart> shopCarts = [];
                if (edm.TemplateType == EdmTemplateType.ShoppingCart)
                {
                    shopCarts = await _shopCartRepository.GetEdmShopCartsAsync([.. members.Select(m => m.id)], edm.GroupBuyId);
                }

                var subject = edm.Subject;

                var template = await _edmTemplateBuilder.BuildAsync(edm);

                foreach (var (id, name, email) in members)
                {
                    var memberTemplate = EdmTemplateBuilder.BuildForMember(template, id, name, shopCarts);

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
            if (ex.Message.Contains("This mail account has sent too many messages"))
            {
                var restClient = new RestClient(_configuration["App:EmailQuotaExceededNotifyUrl"]);
                var restRequest = new RestRequest("", Method.Get);
                await restClient.ExecuteAsync(restRequest);
            }

            _logger.LogException(ex);
            throw;
        }
    }
}
