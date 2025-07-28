using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.PayUni;

[RemoteService(IsEnabled = false)]
public class PayUniAppService : PikachuAppService, IPayUniAppService
{
    public Task ApnNotificationAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task FailureCallbackAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SuccessCallbackAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
