using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public interface ILinePayAppService : IApplicationService
{
    Task<LinePayResponseDto<LinePayPaymentResponseInfoDto>> PaymentRequest(Guid orderId);
    Task<object> ConfirmPayment(long transactionId, string? orderNo);
}
