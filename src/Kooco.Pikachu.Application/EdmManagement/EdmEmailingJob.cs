using Hangfire;
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
    IDataFilter<IMultiTenant> multiTenantFilter
    ) : AsyncBackgroundJob<Guid>, ITransientDependency
{
    public override async Task ExecuteAsync(Guid args)
    {
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
    }
}
