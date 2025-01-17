using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public interface ILinePayAppService : IApplicationService
{
    Task<LinePayPaymentResponseDto> PaymentRequest(Guid orderId);
}
