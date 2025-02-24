using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Orders;

public class CloseOrderBackgroundJob(IOrderAppService orderAppService) : AsyncBackgroundJob<CloseOrderBackgroundJobArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(CloseOrderBackgroundJobArgs args)
    {
        await orderAppService.CloseOrderAsync(args.OrderId);
    }
}
