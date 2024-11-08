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
