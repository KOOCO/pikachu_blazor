using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.EdmManagement;

[AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
public class EdmEmailingJob(
    EdmEmailService edmEmailService,
    IEdmRepository edmRepository,
    IDataFilter<IMultiTenant> multiTenantFilter,
    ILogger<EdmEmailingJob> logger
    ) : AsyncBackgroundJob<Guid>, ITransientDependency
{
    public override async Task ExecuteAsync(Guid args)
    {
        logger.LogInformation("EDM Emailing Job: Starting for ID: {edmId}", args);

        Edm edm;
        using (multiTenantFilter.Disable())
        {
            edm = await edmRepository.GetAsync(args);
        }

        if (edm.TemplateType == EdmTemplateType.ShoppingCart)
        {
            var now = DateTime.Today;

            if (now < edm.StartDate.Date)
            {
                return;
            }

            if (edm.EndDate.HasValue && now > edm.EndDate.Value.Date)
            {
                return;
            }
        }

        await edmEmailService.SendEmailAsync(edm);

        logger.LogInformation("EDM Emailing Job: Finished for ID: {edmId}", args);
    }
}
