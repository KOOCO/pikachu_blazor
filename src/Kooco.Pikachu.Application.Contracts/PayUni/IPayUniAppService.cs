using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PayUni;

public interface IPayUniAppService : IApplicationService
{
    Task SuccessCallbackAsync(CancellationToken cancellationToken = default);
    Task FailureCallbackAsync(CancellationToken cancellationToken = default);
    Task ApnNotificationAsync(CancellationToken cancellationToken = default);
}
