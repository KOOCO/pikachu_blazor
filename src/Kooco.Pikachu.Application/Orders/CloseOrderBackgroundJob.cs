using Kooco.Pikachu.Orders.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Orders;

public class CloseOrderBackgroundJob(IOrderAppService orderAppService) : AsyncBackgroundJob<CloseOrderBackgroundJobArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(CloseOrderBackgroundJobArgs args)
    {
        try
        {
            Logger.LogInformation("{BackgroundJob}: Starting background job", nameof(CloseOrderBackgroundJob));
            await orderAppService.CloseOrdersAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError("{BackgroundJob}: An error occurred during background job", nameof(CloseOrderBackgroundJob));
            Logger.LogException(ex);
        }

        Logger.LogInformation("{BackgroundJob}: Finished background job", nameof(CloseOrderBackgroundJob));
    }
}
