﻿using Asp.Versioning;
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
    [HttpPost("payment-request/{orderId}")]
    public Task<LinePayResponseDto<LinePayPaymentResponseInfoDto>> PaymentRequest(Guid orderId, LinePayPaymentRequestRedirectUrlDto input)
    {
        return linePayAppService.PaymentRequest(orderId, input);
    }

    [HttpPost("confirm/{transactionId}/{orderNo}")]
    public Task<LinePayResponseDto<LinePayConfirmResponseInfoDto>> ConfirmPayment(string transactionId, string? orderNo)
    {
        return linePayAppService.ConfirmPayment(transactionId, orderNo);
    }

    [HttpPost("refund/{refundId}")]
    public Task<LinePayResponseDto<LinePayRefundResponseInfoDto>> ProcessRefund(Guid refundId)
    {
        return linePayAppService.ProcessRefund(refundId);
    }
}
