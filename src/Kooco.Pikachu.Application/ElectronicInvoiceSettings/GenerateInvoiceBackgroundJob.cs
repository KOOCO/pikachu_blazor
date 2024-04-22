
using Hangfire;
using Kooco.Pikachu.Orders;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;
[Queue("automatic-issue-invoice")]
public class GenerateInvoiceBackgroundJob(IElectronicInvoiceAppService orderAppService) : AsyncBackgroundJob<GenerateInvoiceBackgroundJobArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(GenerateInvoiceBackgroundJobArgs args)
    {
        await orderAppService.CreateInvoiceAsync(args.OrderId);
    }
}
