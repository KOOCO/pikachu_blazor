using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.Orders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
