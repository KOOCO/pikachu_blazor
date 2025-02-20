using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public interface ILinePayAppService : IApplicationService
{
    Task<LinePayResponseDto<LinePayPaymentResponseInfoDto>> PaymentRequest(Guid orderId, LinePayPaymentRequestRedirectUrlDto input);
    Task<LinePayResponseDto<LinePayConfirmResponseInfoDto>> ConfirmPayment(string transactionId, string? orderNo);
    Task<LinePayResponseDto<LinePayRefundResponseInfoDto>> ProcessRefund(Guid refundId);
}
