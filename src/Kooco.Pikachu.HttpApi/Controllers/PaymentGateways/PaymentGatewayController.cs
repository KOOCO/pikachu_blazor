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

    [HttpPost("ecpay-serverReplyUrl")]
    [AllowAnonymous]
    public IActionResult EcPayServerReply()
    {
        if (!Request.HasFormContentType) return BadRequest("Invalid content type");

        EcPayStoreData.CVSStoreID = Request.Form["CVSStoreID"];
        
        EcPayStoreData.CVSStoreName = Request.Form["CVSStoreName"];
        
        EcPayStoreData.CVSAddress = Request.Form["CVSAddress"];
        
        EcPayStoreData.CVSOutSide = Request.Form["CVSOutSide"];

        return Redirect("/RedirectionPage");
    }

    [HttpGet("get-ecpayStoreData")]
    public IActionResult GetEcPayStoreData() 
    {
        var ecPayData = new
        {
            CVSStoreID = EcPayStoreData.CVSStoreID,
            CVSStoreName = EcPayStoreData.CVSStoreName,
            CVSAddress = EcPayStoreData.CVSAddress,
            CVSOutSide = EcPayStoreData.CVSOutSide,
            IsDataRetrieved = true
        };

        EcPayStoreData.CVSStoreID = string.Empty;
        EcPayStoreData.CVSStoreName = string.Empty;
        EcPayStoreData.CVSAddress = string.Empty;
        EcPayStoreData.CVSOutSide = string.Empty;

        return Ok(ecPayData);
    } 

    [HttpPost("tcat-serverReplyUrl")]
    [AllowAnonymous]
    public IActionResult TCatServerReply()
    {
        if (!Request.HasFormContentType) return BadRequest("Invalid content type");

        TCatStoreData.outside = Request.Form["outside"];

        TCatStoreData.ship = Request.Form["ship"];

        TCatStoreData.storeaddress = Request.Form["storeaddress"];

        TCatStoreData.storeid = Request.Form["storeid"];

        TCatStoreData.storename = Request.Form["storename"];

        return Redirect("/RedirectionPage");
    }

    [HttpGet("get-tcatStoreData")]
    public IActionResult GetTCatStoreData() 
    {
        var TCatData = new
        {
            outside = TCatStoreData.outside,
            ship = TCatStoreData.ship,
            storeaddress = TCatStoreData.storeaddress,
            storeid = TCatStoreData.storeid,
            storename = TCatStoreData.storename,
            IsDataRetrieved = true
        };

        TCatStoreData.outside = string.Empty;
        TCatStoreData.ship = string.Empty;
        TCatStoreData.storeaddress = string.Empty;
        TCatStoreData.storeid = string.Empty;
        TCatStoreData.storename = string.Empty;

        return Ok(TCatData);
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
