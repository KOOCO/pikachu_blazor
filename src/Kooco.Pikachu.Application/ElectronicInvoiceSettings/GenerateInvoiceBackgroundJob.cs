
using Kooco.Pikachu.Orders;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;

public class GenerateInvoiceBackgroundJob(IElectronicInvoiceAppService orderAppService) : AsyncBackgroundJob<Guid>, ITransientDependency
{
    public override async Task ExecuteAsync(Guid args)
    {
        await orderAppService.CreateInvoiceAsync(args);
    }
}
