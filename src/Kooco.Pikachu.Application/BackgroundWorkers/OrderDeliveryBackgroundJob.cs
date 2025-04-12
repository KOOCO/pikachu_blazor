using Kooco.Pikachu.OBTStatus;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.BackgroundWorkers;
public class OrderDeliveryBackgroundJob(IOBTStatusAppService oBTStatusAppService) : AsyncBackgroundJob<int>, ITransientDependency
{
    public override async Task ExecuteAsync(int args)
    {
        await oBTStatusAppService.UpdateOrderStatusesAsync();
    }
}