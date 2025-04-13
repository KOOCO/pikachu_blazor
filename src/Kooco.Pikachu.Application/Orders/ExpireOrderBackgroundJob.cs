using Kooco.Pikachu.Orders.Interfaces;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Orders
{
	public class ExpireOrderBackgroundJob(IOrderAppService orderAppService) : AsyncBackgroundJob<ExpireOrderBackgroundJobArgs>, ITransientDependency
	{
		public override async Task ExecuteAsync(ExpireOrderBackgroundJobArgs args)
		{
			await orderAppService.ExpireOrderAsync(args.OrderId);
		}
	}
}
