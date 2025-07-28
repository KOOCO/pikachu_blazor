using Asp.Versioning;
using Kooco.Pikachu.PayUni;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.PayUni;

[RemoteService]
[ControllerName("PayUni")]
[Area("app")]
[Route("api/app/pay-uni")]
public class PayUniController(IPayUniAppService payUniAppService) : PikachuController
{
    [HttpPost("apn-notification")]
    public Task ApnNotificationAsync(CancellationToken cancellationToken = default)
    {
        return payUniAppService.ApnNotificationAsync(cancellationToken);
    }

    [HttpPost("callback/failure")]
    public Task FailureCallbackAsync(CancellationToken cancellationToken = default)
    {
        return payUniAppService.FailureCallbackAsync(cancellationToken);
    }

    [HttpPost("callback/success")]
    public Task SuccessCallbackAsync(CancellationToken cancellationToken = default)
    {
        return payUniAppService.SuccessCallbackAsync(cancellationToken);
    }
}