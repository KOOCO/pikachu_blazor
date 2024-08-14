using Asp.Versioning;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PaymentGateways;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Controllers.Orders;

[RemoteService(IsEnabled = true)]
[ControllerName("Orders")]
[Area("app")]
[Route("api/app/orders")]
public class OrderController(
    IOrderAppService _ordersAppService
    ) : AbpController, IOrderAppService
{
    [HttpPost]
    public Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        if (input.CreationTime == DateTime.MinValue) input.CreationTime = DateTime.Now;

        return _ordersAppService.CreateAsync(input);
    }

    [HttpPost("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleCallbackAsync()
    {
        try
        {
            string requestBody = string.Empty;
            // Read the request body into a string
            using (StreamReader reader = new(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var form = await Request.ReadFormAsync();

            bool validRtnCode = int.TryParse(form["RtnCode"], out int rtnCode);
            bool validTradeAmt = int.TryParse(form["TradeAmt"], out int tradeAmt);
            bool validPaymentTypeChargeFee = int.TryParse(form["PaymentTypeChargeFee"], out int paymentTypeChargeFee);
            bool validSimulatePaid = int.TryParse(form["SimulatePaid"], out int simulatePaid);

            var paymentResult = new PaymentResult
            {
                MerchantID = form["MerchantID"],
                MerchantTradeNo = form["MerchantTradeNo"],
                StoreID = form["StoreID"],
                RtnCode = validRtnCode ? rtnCode : 0,
                RtnMsg = form["RtnMsg"],
                TradeNo = form["TradeNo"],
                TradeAmt = validTradeAmt ? tradeAmt : 0,
                PaymentDate = form["PaymentDate"],
                PaymentType = form["PaymentType"],
                PaymentTypeChargeFee = validPaymentTypeChargeFee ? paymentTypeChargeFee : 0,
                TradeDate = form["TradeDate"],
                SimulatePaid = validSimulatePaid ? simulatePaid : 0,
                CheckMacValue = form["CheckMacValue"],
                CustomField1 = form["CustomField1"]
            };

            await _ordersAppService.HandlePaymentAsync(paymentResult);
            return Ok("1|OK");
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("get-order-by-orderNo-extraInfo/{groupBuyId}/{orderNo}/{extraInfo}")]
    public Task<OrderDto> GetOrderAsync(Guid groupBuyId, string orderNo, string extraInfo)
    {
        return _ordersAppService.GetOrderAsync(groupBuyId, orderNo, extraInfo);
    }

    [HttpPut("check-mac-value/{id}/{checkMacValue}")]
    public Task AddCheckMacValueAsync(Guid id, string checkMacValue)
    {
        return _ordersAppService.AddCheckMacValueAsync(id, checkMacValue);
    }

    [HttpPost("payment-gateway-configuration")]
    public Task<PaymentGatewayDto> GetPaymentGatewayConfigurationsAsync([FromBody] Guid id)
    {
        return _ordersAppService.GetPaymentGatewayConfigurationsAsync(id);
    }

    [HttpGet("{id}")]
    public Task<OrderDto> GetAsync(Guid id)
    {
        return _ordersAppService.GetAsync(id);
    }

    [HttpGet()]
    public Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input, bool hideCredentials = false)
    {
        return _ordersAppService.GetListAsync(input, hideCredentials);
    }

    [HttpPut("{id}")]
    public Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input)
    {
        return _ordersAppService.UpdateAsync(id, input);
    }

    [HttpPut("update-shipping-details/{id}")]
    public Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
    {
        return _ordersAppService.UpdateShippingDetails(id, input);
    }

    [HttpGet("get-with-details/{id}")]
    public Task<OrderDto> GetWithDetailsAsync(Guid id)
    {
        return _ordersAppService.GetWithDetailsAsync(id);
    }

    [HttpPost("add-store-comments/{id}/{comment}")]
    public Task AddStoreCommentAsync(Guid id, string comment)
    {
        return _ordersAppService.AddStoreCommentAsync(id, comment);
    }

    [HttpPut("update-store-comment/{id}/{commentId}/{comment}")]
    public Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment)
    {
        return _ordersAppService.UpdateStoreCommentAsync(id, commentId, comment);
    }

    [HttpPost("handle-payment")]
    public Task HandlePaymentAsync(PaymentResult paymentResult)
    {
        return _ordersAppService.HandlePaymentAsync(paymentResult);
    }

    [HttpPut("update-order-items/{id}")]
    public Task UpdateOrderItemsAsync(Guid id, List<UpdateOrderItemDto> orderItems)
    {
        return _ordersAppService.UpdateOrderItemsAsync(id, orderItems);
    }

    [HttpPut("cancel-order/{id}")]
    public Task CancelOrderAsync(Guid id)
    {
        return _ordersAppService.CancelOrderAsync(id);
    }

    [HttpGet("get-return-list")]
    public Task<PagedResultDto<OrderDto>> GetReturnListAsync(GetOrderListDto input)
    {
        return _ordersAppService.GetReturnListAsync(input);
    }

    [HttpPut("update-return-status/{id}/{orderReturnStatus}")]
    public Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus)
    {
        return _ordersAppService.ChangeReturnStatusAsync(id, orderReturnStatus);
    }

    [HttpGet("get-list-as-excel")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetOrderListDto input)
    {
        return _ordersAppService.GetListAsExcelFileAsync(input);
    }

    [HttpPost("merge-orders")]
    public Task<OrderDto> MergeOrdersAsync(List<Guid> Ids)
    {
        return _ordersAppService.MergeOrdersAsync(Ids);
    }

    [HttpPost("split-orders")]
    public Task<OrderDto> SplitOrderAsync(List<Guid> OrderItemIds, Guid OrderId)
    {
        return _ordersAppService.SplitOrderAsync(OrderItemIds, OrderId);
    }

    [HttpPost("exchange-order")]
    public Task ExchangeOrderAsync(Guid id)
    {
        return _ordersAppService.ExchangeOrderAsync(id);
    }

    [HttpGet("get-tenant-order-list")]
    public Task<PagedResultDto<OrderDto>> GetTenantOrderListAsync(GetOrderListDto input)
    {
        return _ordersAppService.GetTenantOrderListAsync(input);
    }

    [HttpGet("get-reconciliation-list")]
    public Task<PagedResultDto<OrderDto>> GetReconciliationListAsync(GetOrderListDto input)
    {
        return _ordersAppService.GetReconciliationListAsync(input);
    }

    [HttpGet("get-reconciliation-as-excel")]
    public Task<IRemoteStreamContent> GetReconciliationListAsExcelFileAsync(GetOrderListDto input)
    {
        return _ordersAppService.GetReconciliationListAsExcelFileAsync(input);
    }
    [HttpPost("order-shipped")]
    public Task<OrderDto> OrderShipped(Guid id)
    {
        return _ordersAppService.OrderShipped(id);
    }
    [HttpPost("order-closed")]
    public Task<OrderDto> OrderClosed(Guid id)
    {
        return _ordersAppService.OrderClosed(id);
    }
    [HttpPost("order-completed")]
    public Task<OrderDto> OrderComplete(Guid id)
    {
        return _ordersAppService.OrderComplete(id);
    }
    
    public Task VoidInvoice(Guid id, string reason)
    {
        return _ordersAppService.VoidInvoice(id, reason);
    }
    [HttpGet("void-list")]
    public Task<PagedResultDto<OrderDto>> GetVoidListAsync(GetOrderListDto input)
    {
        return _ordersAppService.GetVoidListAsync(input);
    }
    
    public Task CreditNoteInvoice(Guid id, string reason)
    {
        return _ordersAppService.CreditNoteInvoice(id, reason);
    }

    public Task<OrderDto> RefundOrderItems(List<Guid> OrderItemIds, Guid OrderId)
    {
       return _ordersAppService.RefundOrderItems(OrderItemIds, OrderId);
    }

    public Task RefundAmountAsync(double amount, Guid OrderId)
    {
        return _ordersAppService.RefundAmountAsync(amount, OrderId);
    }

    public Task<PagedResultDto<OrderDto>> GetReportListAsync(GetOrderListDto input, bool hideCredentials = false)
    {
        return _ordersAppService.GetReportListAsync(input, hideCredentials);
    }
    [HttpPost("return-order")]
    public Task ReturnOrderAsync(Guid id)
    {
        return _ordersAppService.ReturnOrderAsync(id);
    }

    [HttpPost("update-orders-if-IsEnterpricePurchase")]
    public Task UpdateOrdersIfIsEnterpricePurchaseAsync(Guid groupBuyId)
    {
        return _ordersAppService.UpdateOrdersIfIsEnterpricePurchaseAsync(groupBuyId);
    }

    public Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync()
    {
        throw new NotImplementedException();
    }
}
