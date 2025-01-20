using Asp.Versioning;
using Kooco.Pikachu.PaymentGateways.LinePay;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.PaymentGateways.LinePay;

[RemoteService(IsEnabled = true)]
[ControllerName("LinePay")]
[Area("app")]
[Route("api/app/line-pay")]
public class LinePayController(ILinePayAppService linePayAppService) : PikachuController, ILinePayAppService
{
    [HttpPost("confirm/{transactionId}/{orderNo}")]
    public Task<object> ConfirmPayment(string transactionId, string? orderNo)
    {
        return linePayAppService.ConfirmPayment(transactionId, orderNo);
    }

    [HttpPost("payment-request/{orderId}")]
    public Task<LinePayResponseDto<LinePayPaymentResponseInfoDto>> PaymentRequest(Guid orderId)
    {
        return linePayAppService.PaymentRequest(orderId);
    }

    [HttpPost("refund/{orderId}")]
    public Task<object> ProcessRefund(Guid orderId)
    {
        return linePayAppService.ProcessRefund(orderId);
    }
}
