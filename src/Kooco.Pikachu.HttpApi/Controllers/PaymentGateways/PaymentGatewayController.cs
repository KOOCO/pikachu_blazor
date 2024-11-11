using Asp.Versioning;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.PaymentGateways;

[RemoteService(IsEnabled = true)]
[ControllerName("PaymentGateways")]
[Area("app")]
[Route("api/app/payment-gateways")]
public class PaymentGatewayController(
    IPaymentGatewayAppService _paymentGatewayAppService
    ) : AbpController, IPaymentGatewayAppService
{
    [HttpGet]
    public Task<List<PaymentGatewayDto>> GetAllAsync()
    {
        return _paymentGatewayAppService.GetAllAsync();
    }

    [HttpPost("ecpay-serverReplyUrlWith/{UniqueId}")]
    [AllowAnonymous]
    public IActionResult EcPayServerReplyQS(Guid UniqueId)
    {
        if (!Request.HasFormContentType) return BadRequest("Invalid content type");

        EcPayStoreData ecpayStoreData = new()
        {
            CVSStoreID = Request.Form["CVSStoreID"],
            CVSStoreName = Request.Form["CVSStoreName"],
            CVSAddress = Request.Form["CVSAddress"],
            CVSOutSide = Request.Form["CVSOutSide"],
            UniqueId = UniqueId.ToString()
        };

        RecordEcPay.UniqueEcPayData ??= [];

        RecordEcPay.UniqueEcPayData.TryAdd(UniqueId, ecpayStoreData);

        return Redirect("/RedirectionPage");
    }  

    [HttpGet("get-ecpayStoreData/{UniqueId}")]
    public IActionResult GetEcPayStoreData(Guid UniqueId)
    {
        if (RecordEcPay.UniqueEcPayData.TryGetValue(UniqueId, out EcPayStoreData? ecpayStoreData))
        {
            RecordEcPay.UniqueEcPayData.Remove(UniqueId);

            return Ok(new {
                CVSStoreID = ecpayStoreData.CVSStoreID,
                CVSStoreName = ecpayStoreData.CVSStoreName,
                CVSAddress = ecpayStoreData.CVSAddress,
                CVSOutSide = ecpayStoreData.CVSOutSide,
                UniqueId = UniqueId.ToString(),
                IsDataRetrieved = true
            });
        }

        return Ok($"Data not available with UniqueId: {UniqueId}");
    }

    [HttpPost("tcat-serverReplyUrl/{UniqueId}")]
    [AllowAnonymous]
    public IActionResult TCatServerReply(Guid UniqueId)
    {
        if (!Request.HasFormContentType) return BadRequest("Invalid content type");

        TCatStoreData tcatStoreData = new()
        {
            outside = Request.Form["outside"],
            ship = Request.Form["ship"],
            storeaddress = Request.Form["storeaddress"],
            storeid = Request.Form["storeid"],
            storename = Request.Form["storename"],
            UniqueId = UniqueId.ToString()
        };

        RecordEcPay.UniqueTCatStoreData ??= [];

        RecordEcPay.UniqueTCatStoreData.TryAdd(UniqueId, tcatStoreData);

        return Redirect("/RedirectionPage");
    }

    [HttpGet("get-tcatStoreData/{UniqueId}")]
    public IActionResult GetTCatStoreData(Guid UniqueId) 
    {
        if (RecordEcPay.UniqueTCatStoreData.TryGetValue(UniqueId, out TCatStoreData? tcatStoreData))
        {
            RecordEcPay.UniqueTCatStoreData.Remove(UniqueId);

            return Ok(new
            {
                outside = tcatStoreData.outside,
                ship = tcatStoreData.ship,
                storeaddress = tcatStoreData.storeaddress,
                storeid = tcatStoreData.storeid,
                storename = tcatStoreData.storename,
                UniqueId = UniqueId.ToString(),
                IsDataRetrieved = true
            });
        }

        return Ok($"Data not available with UniqueId: {UniqueId}");
    }

    [HttpPut("china-trust")]
    public Task UpdateChinaTrustAsync(UpdateChinaTrustDto input)
    {
        return _paymentGatewayAppService.UpdateChinaTrustAsync(input);
    }

    [HttpPut("ec-pay")]
    public Task UpdateEcPayAsync(UpdateEcPayDto input)
    {
        return _paymentGatewayAppService.UpdateEcPayAsync(input);
    }

    [HttpPut("line-pay")]
    public Task UpdateLinePayAsync(UpdateLinePayDto input)
    {
        return _paymentGatewayAppService.UpdateLinePayAsync(input);
    }

    #region Private Functions
    private string CloseWindowsScript()
    {
        return @"
            
            <script>
                debugger;
                if (confirm('please go back to main checkout page to continue the order placement.')) 
                { 
                    setTimeout(() => {
                         window.close();
                    }, 500);
                }

                else 
                {
                    setTimeout(() => {
                         window.close();
                    }, 500);
                }
            </script>
        ";
    }
    #endregion
}
