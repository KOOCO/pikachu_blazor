using Kooco.Pikachu.OBTStatus;
using Kooco.Pikachu.OrderDeliveries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.BackgroundWorkers
{
	public class OrderDeliveryBackgroundJob : AsyncBackgroundJob<int>, ITransientDependency
	{
		private readonly IOBTStatusAppService _oBTStatusAppService;

		public OrderDeliveryBackgroundJob(IOBTStatusAppService oBTStatusAppService)
		{
			_oBTStatusAppService = oBTStatusAppService;
		}

		

		public override async Task ExecuteAsync(int args)
		{
			await _oBTStatusAppService.UpdateOrderStatusesAsync();
		}
	}
}
