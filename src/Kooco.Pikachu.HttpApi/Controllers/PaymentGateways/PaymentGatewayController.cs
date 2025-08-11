using Asp.Versioning;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost("ecpay-serverReplyUrl/{UniqueId}/{GroupBuyId}")]
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public IActionResult EcPayServerReplyQS(Guid UniqueId, Guid GroupBuyId)
    {
        if (!Request.HasFormContentType) return BadRequest("Invalid content type");

        EcPayStoreData ecpayStoreData = new()
        {
            CVSStoreID = Request.Form["CVSStoreID"],
            CVSStoreName = Request.Form["CVSStoreName"],
            CVSAddress = Request.Form["CVSAddress"],
            CVSOutSide = Request.Form["CVSOutSide"],
            GroupBuyId = GroupBuyId.ToString(),
            UniqueId = UniqueId.ToString()
        };

        RecordEcPay.UniqueEcPayData ??= [];

        RecordEcPay.UniqueEcPayData.TryAdd(UniqueId, ecpayStoreData);

        string htmlContent = @"
            <script>                
                window.close();
            </script>
        ";

        return Content(htmlContent, "text/html");
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
                GroupBuyId = ecpayStoreData.GroupBuyId,
                UniqueId = UniqueId.ToString(),
                IsDataRetrieved = true
            });
        }

        return Ok($"Data not available with UniqueId: {UniqueId}");
    }

    [HttpPost("tcat-serverReplyUrl/{UniqueId}/{GroupBuyId}")]
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public IActionResult TCatServerReply(Guid UniqueId, Guid GroupBuyId)
    {
        if (!Request.HasFormContentType) return BadRequest("Invalid content type");

        TCatStoreData tcatStoreData = new()
        {
            outside = Request.Form["outside"],
            ship = Request.Form["ship"],
            storeaddress = Request.Form["storeaddress"],
            storeid = Request.Form["storeid"],
            storename = Request.Form["storename"],
            GroupBuyId = GroupBuyId.ToString(),
            UniqueId = UniqueId.ToString()
        };

        RecordEcPay.UniqueTCatStoreData ??= [];

        RecordEcPay.UniqueTCatStoreData.TryAdd(UniqueId, tcatStoreData);

        string htmlContent = @"
            <script>                
                window.close();
            </script>
        ";

        return Content(htmlContent, "text/html");
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
                GroupBuyId = tcatStoreData.GroupBuyId,
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
    [HttpPut("payment-deadline")]
    public Task UpdateOrderValidityAsync(UpdateOrderValidityDto input)
    {
        return _paymentGatewayAppService.UpdateOrderValidityAsync(input);
    }

    [HttpGet("get-creditCheckCode")]
    public Task<string?> GetCreditCheckCodeAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("line-pay")]
    public Task<PaymentGatewayDto?> GetLinePayAsync(bool decrypt = false)
    {
        return _paymentGatewayAppService.GetLinePayAsync(decrypt);
    }

    [HttpGet("manual-bank-transfer")]
    public Task<ManualBankTransferDto?> GetManualBankTransferAsync()
    {
        return _paymentGatewayAppService.GetManualBankTransferAsync();
    }

    [HttpPut("manual-bank-transfer")]
    public Task UpdateManualBankTransferAsync(UpdateManualBankTransferDto input)
    {
        return _paymentGatewayAppService.UpdateManualBankTransferAsync(input);
    }


    [HttpGet("ecpay")]
    public Task<PaymentGatewayDto?> GetEcPayAsync(bool decrypt = false)
    {
        return _paymentGatewayAppService.GetEcPayAsync(decrypt);
    }

    [HttpGet("ecpay-list")]
    public Task<List<PaymentGatewayDto>> GetAllEcPayAsync(bool decrypt = false)
    {
        return _paymentGatewayAppService.GetAllEcPayAsync(decrypt);
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
