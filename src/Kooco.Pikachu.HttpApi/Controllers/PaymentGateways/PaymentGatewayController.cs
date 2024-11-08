using Asp.Versioning;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
    public IActionResult PostAsync()
    {
        EcPayStoreData.CVSStoreID = Request.Form.TryGetValue("CVSStoreID", out StringValues cvsStoreId) ? cvsStoreId.ToString() : string.Empty;

        EcPayStoreData.CVSStoreName = Request.Form.TryGetValue("CVSStoreName", out StringValues cvsStoreName) ? cvsStoreName.ToString() : string.Empty;

        EcPayStoreData.CVSAddress = Request.Form.TryGetValue("CVSAddress", out StringValues cvsAddress) ? cvsAddress.ToString() : string.Empty;

        EcPayStoreData.CVSOutSide = Request.Form.TryGetValue("CVSOutSide", out StringValues cvsOutSide) ? cvsOutSide.ToString() : string.Empty;

        return Content(CloseWindowsScript(), "text/html");
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
    public IActionResult TCatPostAsync()
    {
        TCatStoreData.outside = Request.Form.TryGetValue("outside", out StringValues outside) ? outside.ToString() : string.Empty;

        TCatStoreData.ship = Request.Form.TryGetValue("ship", out StringValues ship) ? ship.ToString() : string.Empty;

        TCatStoreData.storeaddress = Request.Form.TryGetValue("storeaddress", out StringValues stroreaddress) ? stroreaddress.ToString() : string.Empty;

        TCatStoreData.storeid = Request.Form.TryGetValue("storeid", out StringValues storeid) ? storeid.ToString() : string.Empty;

        TCatStoreData.storename = Request.Form.TryGetValue("storename", out StringValues storename) ? storename.ToString() : string.Empty;

        return Content(CloseWindowsScript(), "text/html");
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
