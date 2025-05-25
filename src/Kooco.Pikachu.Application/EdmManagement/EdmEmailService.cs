using Hangfire;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.ShopCarts;
using Kooco.Pikachu.Tenants;
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
    private readonly ITenantSettingsAppService _tenantSettingsAppService;
    private readonly IShopCartRepository _shopCartRepository;

    public EdmEmailService(
        IEdmRepository edmRepository,
        IEmailSender emailSender,
        IMemberRepository memberRepository,
        ICurrentTenant currentTenant,
        ICampaignRepository campaignRepository,
        ILogger<EdmEmailService> logger,
        ITenantSettingsAppService tenantSettingsAppService,
        IShopCartRepository shopCartRepository
        )
    {
        _edmRepository = edmRepository;
        _emailSender = emailSender;
        _memberRepository = memberRepository;
        _currentTenant = currentTenant;
        _campaignRepository = campaignRepository;
        _logger = logger;
        _tenantSettingsAppService = tenantSettingsAppService;
        _shopCartRepository = shopCartRepository;
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

                List<ShopCart> shopCarts = [];
                if (edm.TemplateType == EdmTemplateType.ShoppingCart)
                {
                    shopCarts = await _shopCartRepository.GetEdmShopCartsAsync([.. members.Select(m => m.id)], edm.GroupBuyId);
                }

                await _edmRepository.EnsurePropertyLoadedAsync(edm, edm => edm.GroupBuy);

                var subject = edm.Subject;

                var tenantSettings = await _tenantSettingsAppService.FirstOrDefaultAsync();

                var template = await EdmTemplateBuilder.Build(edm, tenantSettings, _campaignRepository);

                foreach (var (id, name, email) in members)
                {
                    var memberTemplate = template.Replace("{{MemberName}}", name);
                    if (edm.TemplateType == EdmTemplateType.ShoppingCart)
                    {
                        var memberCartItems = shopCarts.SelectMany(sc => sc.CartItems);
                        if (!memberCartItems.Any()) continue;

                        var shopCartItemsTemplatePath = Path.Combine("wwwroot", "EmailTemplates", "Edms", "edm_shopping_cart_items.html");
                        var shopCartItemsTemplate = File.ReadAllText(shopCartItemsTemplatePath);
                        var memberItemsTemplate = string.Join("", memberCartItems.Select(ci =>
                            shopCartItemsTemplate
                                .Replace("{{ItemName}}", ci.Item?.ItemName ?? ci.SetItem?.SetItemName)
                                .Replace("{{ItemDetails}}", "")
                                .Replace("{{UnitPrice}}", ci.UnitPrice.ToString("N2"))
                                .Replace("{{ItemQuantity}}", ci.Quantity.ToString("N0"))
                                .Replace("{{ItemTotal}}", (ci.UnitPrice * ci.Quantity).ToString("N2"))));

                        memberTemplate = memberTemplate.Replace("{{edm_shopping_cart_items}}", memberItemsTemplate);
                    }

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
