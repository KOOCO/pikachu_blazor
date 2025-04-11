
using Hangfire;
using Kooco.Pikachu.Orders.Interfaces;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;

[Queue("automatic-issue-invoice")]
public class GenerateInvoiceBackgroundJob : AsyncBackgroundJob<GenerateInvoiceBackgroundJobArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(GenerateInvoiceBackgroundJobArgs args)
    {
        await OrderInvoiceAppService.CreateInvoiceAsync(args.OrderId);
    }

    public IOrderInvoiceAppService OrderInvoiceAppService { get; set; }
}