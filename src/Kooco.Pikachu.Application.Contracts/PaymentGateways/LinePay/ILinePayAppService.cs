using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public interface ILinePayAppService : IApplicationService
{
    Task<LinePayResponseDto<LinePayPaymentResponseInfoDto>> PaymentRequest(Guid orderId);
    Task<object> ConfirmPayment(string transactionId, string? orderNo);
    Task<object> ProcessRefund(Guid orderId);
}
