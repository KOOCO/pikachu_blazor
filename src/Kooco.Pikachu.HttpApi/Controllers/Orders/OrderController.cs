using Asp.Versioning;
using ECPay.Payment.Integration;
using Kooco.Pikachu.Controllers.Common;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.StoreLogisticOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Controllers.Orders;

[RemoteService(IsEnabled = true)]
[ControllerName("Orders")]
[Area("app")]
[Route("api/app/orders")]
public class OrderController : AbpController
{
    #region Inject
    private readonly IOrderAppService _ordersAppService;
    private readonly IConfiguration _Configuration;
    private readonly IGroupBuyAppService _GroupBuyAppService;
    private readonly IStoreLogisticsOrderAppService _SLOAppservice;
    #endregion

    #region Constructor
    public OrderController(
        IOrderAppService ordersAppService,
        IConfiguration Configuration,
        IGroupBuyAppService GroupBuyAppService,
        IStoreLogisticsOrderAppService SLOAppservice
    )
    {
        _ordersAppService = ordersAppService;
        _Configuration = Configuration;
        _GroupBuyAppService = GroupBuyAppService;
        _SLOAppservice = SLOAppservice;
    }
    #endregion

    #region Methods
    [HttpGet("gen-CM")]
    public IActionResult GenCheckMac()
    {
        Dictionary<string, string> keyValuePairs = [];

        keyValuePairs.Add("MerchantID", "2000132");
        keyValuePairs.Add("AllPayLogisticsID", "3136268");
        //keyValuePairs.Add("TimeStamp", timeStamp.ToString());

        IOrderedEnumerable<KeyValuePair<string, string>> sortedParameters = keyValuePairs.OrderBy(p => p.Key);

        string requestString = string.Join("&", sortedParameters.Select(p => $"{p.Key}={p.Value}"));

        requestString = $"HashKey=5294y06JbISpM5x9&{requestString}&HashIV=v77hoKGq4kWxNNIS";

        string urlEncodedData = HttpUtility.UrlEncode(requestString);

        string lowercaseData = urlEncodedData.ToLower();

        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(lowercaseData);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return Ok(sb.ToString());
        }
    }



    [HttpPost]
    public Task<OrderDto> CreateAsync(CreateUpdateOrderDto input)
    {
        if (input.CreationTime == DateTime.MinValue) input.CreationTime = DateTime.Now;

        return _ordersAppService.CreateAsync(input);
    }

    [HttpPost("find-status")]
    public async Task<IActionResult> FindStatusAsync()
    {
        string status = await _SLOAppservice.FindStatusAsync();

        return Ok();
    }

    [HttpGet("proceed-to-checkout")]
    public async Task<IActionResult> ProceedToCheckout(Guid orderId, string clientBackUrl, PaymentMethods paymentMethodsValue, bool isInstallments = false)
    {
        // Delegate to OrderAppService for payment processing
        // The Payment Strategy pattern is now implemented within the application layer
        var result = await _ordersAppService.ProceedToCheckoutAsync(orderId, clientBackUrl, paymentMethodsValue, isInstallments);
        
        // Convert string result to appropriate HTTP response
        if (string.IsNullOrEmpty(result))
        {
            return BadRequest("Failed to process payment checkout");
        }
        
        // If result contains HTML (payment form), return as content
        if (result.Contains("<form"))
        {
            return Content(result, "text/html");
        }
        
        // Otherwise, return as JSON
        return Ok(new { redirectUrl = result });
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

            IFormCollection form = await Request.ReadFormAsync();

            bool validRtnCode = int.TryParse(form["RtnCode"], out int rtnCode);
            bool validGWSR = int.TryParse(form["gwsr"], out int gwsr);
            bool validTradeAmt = int.TryParse(form["TradeAmt"], out int tradeAmt);
            bool validPaymentTypeChargeFee = int.TryParse(form["PaymentTypeChargeFee"], out int paymentTypeChargeFee);
            bool validSimulatePaid = int.TryParse(form["SimulatePaid"], out int simulatePaid);

            PaymentResult paymentResult = new()
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
                CustomField1 = form["CustomField1"],
                GWSR = validGWSR ? gwsr : 0
            };

            await _ordersAppService.HandlePaymentAsync(paymentResult);

            return Ok("1|OK");
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost("ecpay-logisticsStatus-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> EcpayLogisticsStatusCallbackAsync()
    {
        try
        {
            bool validRtnCode = int.TryParse(Request.Form["RtnCode"], out int rtnCode);
            bool validGoodsAmount = int.TryParse(Request.Form["GoodsAmount"], out int goodsAmount);

            EcpayRequest ecpayRequest = new()
            {
                MerchantID = Request.Form["MerchantID"],
                MerchantTradeNo = Request.Form["MerchantTradeNo"],
                RtnCode = validRtnCode ? rtnCode : 0,
                RtnMsg = Request.Form["RtnMsg"],
                LogisticsType = Request.Form["LogisticType"],
                LogisticsSubType = Request.Form["LogisticsSubType"],
                GoodsAmount = validGoodsAmount ? goodsAmount : 0,
                UpdateStatusDate = Request.Form["UpdateStatusDate"],
                ReceiverName = Request.Form["ReceiverName"],
                ReceiverPhone = Request.Form["ReceiverPhone"],
                ReceiverCellPhone = Request.Form["ReceiverCellPhone"],
                ReceiverEmail = Request.Form["ReceiverEmail"],
                ReceiverAddress = Request.Form["ReceiverAddress"],
                CVSPaymentNo = Request.Form["CVSPaymentNo"],
                CVSValidationNo = Request.Form["CVSValidationNo"],
                BookingNote = Request.Form["BookingNote"],
                CheckMacValue = Request.Form["CheckMacValue"],
            };

            await _ordersAppService.UpdateLogisticStatusAsync(ecpayRequest.MerchantTradeNo!, ecpayRequest.RtnMsg!, ecpayRequest.RtnCode);

            return Ok("1|OK");
        }
        catch
        {
            return Ok("0| ErrorMessage");
        }
    }

    [HttpGet("get-order-by-orderNo-extraInfo/{groupBuyId}/{orderNo}/{extraInfo}")]
    public Task<OrderDto> GetOrderAsync(Guid groupBuyId, string orderNo, string extraInfo)
    {
        return _ordersAppService.GetOrderAsync(groupBuyId, orderNo, extraInfo);
    }

    [HttpPost("update-merchantTradeNo")]
    public Task<OrderDto> UpdateMerchantTradeNoAsync([FromBody] OrderPaymentMethodRequest request)
    {
        return _ordersAppService.UpdateMerchantTradeNoAsync(request);
    }

    [HttpPost("update-order-paymentMethod")]
    public Task<OrderDto> UpdateOrderPaymentMethodAsync([FromBody] OrderPaymentMethodRequest request)
    {
        return _ordersAppService.UpdateOrderPaymentMethodAsync(request);
    }

    [HttpPut("check-mac-value/{id}/{checkMacValue}")]
    public Task AddCheckMacValueAsync(Guid id, string checkMacValue)
    {
        return _ordersAppService.AddCheckMacValueAsync(id, checkMacValue);
    }

    [HttpPut("add-values/{id}/{checkMacValue}/{merchantTradeNo}")]
    public Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo)
    {
        return _ordersAppService.AddValuesAsync(id, checkMacValue, merchantTradeNo);
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
    public Task<string> HandlePaymentAsync(PaymentResult paymentResult)
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
    public Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus, bool isRefund)
    {
        return ((IOrderLogisticsService)_ordersAppService).ChangeReturnStatusAsync(id, orderReturnStatus, isRefund);
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
        return ((IOrderLogisticsService)_ordersAppService).ExchangeOrderAsync(id);
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
        return ((IOrderStatusService)_ordersAppService).OrderShipped(id);
    }
    [HttpPost("order-closed")]
    public Task<OrderDto> OrderClosed(Guid id)
    {
        return ((IOrderStatusService)_ordersAppService).OrderClosed(id);
    }
    [HttpPost("order-completed")]
    public Task<OrderDto> OrderComplete(Guid id)
    {
        return ((IOrderStatusService)_ordersAppService).OrderComplete(id);
    }

    [HttpPost("void-invoice")]
    public Task VoidInvoice(Guid id, string reason)
    {
        return _ordersAppService.VoidInvoice(id, reason);
    }
    [HttpGet("void-list")]
    public Task<PagedResultDto<OrderDto>> GetVoidListAsync(GetOrderListDto input)
    {
        return _ordersAppService.GetVoidListAsync(input);
    }

    [HttpPost("credit-note-invoice")]
    public Task CreditNoteInvoice(Guid id, string reason)
    {
        return ((IOrderInvoiceService)_ordersAppService).CreditNoteInvoice(id, reason);
    }

    [HttpPost("refund-order-items")]
    public Task<OrderDto> RefundOrderItems(List<Guid> OrderItemIds, Guid OrderId)
    {
        return _ordersAppService.RefundOrderItems(OrderItemIds, OrderId);
    }

    [HttpPost("refund-amount")]
    public Task RefundAmountAsync(double amount, Guid OrderId)
    {
        return _ordersAppService.RefundAmountAsync(amount, OrderId);
    }

    [HttpGet("get-report-list")]
    public Task<PagedResultDto<GroupBuyReportOrderDto>> GetReportListAsync(GetOrderListDto input, bool hideCredentials = false)
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

    [HttpGet("get-total-delivery-tempCount")]
    public Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync()
    {
        throw new NotImplementedException();
    }
    [HttpPost("order-to-be-shipped")]
    public Task<OrderDto> OrderToBeShipped(Guid id)
    {
        return _ordersAppService.OrderToBeShipped(id);
    }
    [HttpGet("get-order-status-amount")]
    public Task<(decimal PaidAmount, decimal UnpaidAmount, decimal RefundedAmount)> GetOrderStatusAmountsAsync(Guid UserId)
    {
        return _ordersAppService.GetOrderStatusAmountsAsync(UserId);
    }
    [HttpGet("get-order-status-count")]
    public Task<(int Open, int Exchange, int Return)> GetOrderStatusCountsAsync(Guid userId)
    {
        return _ordersAppService.GetOrderStatusCountsAsync(userId);
    }

    [HttpPost("update-logisticsStatus")]
    public Task UpdateLogisticStatusAsync(string merchantTradeNo, string rtnMsg, int rtnCode = 0)
    {
        return _ordersAppService.UpdateLogisticStatusAsync(merchantTradeNo, rtnMsg);
    }

    [HttpPost("change-order-status/{id}/{status}")]
    public Task<OrderDto> ChangeOrderStatus(Guid id, ShippingStatus status)
    {
        return _ordersAppService.ChangeOrderStatus(id, status);
    }

    [HttpGet("generate-merchantTradeNo")]
    public string GenerateMerchantTradeNo(string orderNo)
    {
        throw new NotImplementedException();
    }

    [HttpPut("add-values/{id}/{checkMacValue}/{merchantTradeNo}/{paymentMethod}")]
    public Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo, PaymentMethods? paymentMethod = null)
    {
        throw new NotImplementedException();
    }

    [HttpPost("order-expire")]
    public Task ExpireOrderAsync(Guid OrderId)
    {
        return _ordersAppService.ExpireOrderAsync(OrderId);
    }

    [HttpGet("order-logs")]
    public Task<List<OrderHistoryDto>> GetOrderLogsAsync(Guid orderId)
    {
        return _ordersAppService.GetOrderLogsAsync(orderId);
    }

    [HttpPost("order-deliveries-and-invoice/{orderId}")]
    public Task CreateOrderDeliveriesAndInvoiceAsync(Guid orderId)
    {
        return _ordersAppService.CreateOrderDeliveriesAndInvoiceAsync(orderId);
    }

    [HttpPost("close-orders")]
    public Task CloseOrdersAsync()
    {
        return _ordersAppService.CloseOrdersAsync();
    }

    [HttpGet("return-notification-count")]
    public Task<long> GetReturnOrderNotificationCount()
    {
        return _ordersAppService.GetReturnOrderNotificationCount();
    }

    [HttpGet("get-order-id")]
    [AllowAnonymous]
    public Task<Guid> GetOrderIdAsync(string orderNo)
    {
        return _ordersAppService.GetOrderIdAsync(orderNo);
    }

    // IOrderNotificationService HTTP endpoints
    [HttpPost("send-order-confirmation-email/{orderId}")]
    public Task SendOrderConfirmationEmailAsync(Guid orderId)
    {
        return _ordersAppService.SendOrderConfirmationEmailAsync(orderId);
    }

    [HttpPost("send-payment-confirmation-email/{orderId}")]
    public Task SendPaymentConfirmationEmailAsync(Guid orderId)
    {
        return _ordersAppService.SendPaymentConfirmationEmailAsync(orderId);
    }

    [HttpPost("send-shipping-notification-email/{orderId}")]
    public Task SendShippingNotificationEmailAsync(Guid orderId)
    {
        return _ordersAppService.SendShippingNotificationEmailAsync(orderId);
    }

    [HttpPost("send-order-completion-email/{orderId}")]
    public Task SendOrderCompletionEmailAsync(Guid orderId)
    {
        return _ordersAppService.SendOrderCompletionEmailAsync(orderId);
    }

    [HttpPost("send-order-cancellation-email/{orderId}")]
    public Task SendOrderCancellationEmailAsync(Guid orderId)
    {
        return _ordersAppService.SendOrderCancellationEmailAsync(orderId);
    }

    [HttpPost("send-refund-notification-email/{orderId}")]
    public Task SendRefundNotificationEmailAsync(Guid orderId)
    {
        return _ordersAppService.SendRefundNotificationEmailAsync(orderId);
    }

    [HttpPost("send-email/{orderId}")]
    public Task SendEmailAsync(Guid orderId)
    {
        return _ordersAppService.SendEmailAsync(orderId);
    }

    // IOrderInvoiceService HTTP endpoints
    [HttpPost("generate-invoice/{orderId}")]
    public Task<OrderInvoiceDto> GenerateInvoiceAsync(Guid orderId)
    {
        return _ordersAppService.GenerateInvoiceAsync(orderId);
    }

    [HttpPut("update-invoice-details/{orderId}")]
    public Task UpdateInvoiceDetailsAsync(Guid orderId, string invoiceNumber, string invoiceDetails)
    {
        return _ordersAppService.UpdateInvoiceDetailsAsync(orderId, invoiceNumber, invoiceDetails);
    }

    [HttpPost("send-invoice/{orderId}")]
    public Task SendInvoiceAsync(Guid orderId)
    {
        return _ordersAppService.SendInvoiceAsync(orderId);
    }

    [HttpGet("get-invoice-status/{orderId}")]
    public Task<InvoiceStatusDto> GetInvoiceStatusAsync(Guid orderId)
    {
        return _ordersAppService.GetInvoiceStatusAsync(orderId);
    }

    [HttpPost("process-electronic-invoice/{orderId}")]
    public Task ProcessElectronicInvoiceAsync(Guid orderId)
    {
        return _ordersAppService.ProcessElectronicInvoiceAsync(orderId);
    }

    // IOrderInventoryService HTTP endpoints
    [HttpPost("reserve-inventory/{orderId}")]
    public Task ReserveInventoryAsync(Guid orderId)
    {
        return _ordersAppService.ReserveInventoryAsync(orderId);
    }

    [HttpPost("release-inventory/{orderId}")]
    public Task ReleaseInventoryAsync(Guid orderId)
    {
        return _ordersAppService.ReleaseInventoryAsync(orderId);
    }

    [HttpPost("validate-inventory-availability")]
    public Task<bool> ValidateInventoryAvailabilityAsync(List<UpdateOrderItemDto> orderItems)
    {
        return _ordersAppService.ValidateInventoryAvailabilityAsync(orderItems);
    }

    [HttpPost("restore-inventory/{orderId}")]
    public Task RestoreInventoryAsync(Guid orderId)
    {
        return _ordersAppService.RestoreInventoryAsync(orderId);
    }

    [HttpPost("adjust-inventory-for-modification/{orderId}")]
    public Task AdjustInventoryForOrderModificationAsync(Guid orderId, List<UpdateOrderItemDto> newOrderItems)
    {
        return _ordersAppService.AdjustInventoryForOrderModificationAsync(orderId, newOrderItems);
    }

    // IOrderLogisticsService HTTP endpoints
    [HttpPost("create-order-deliveries/{orderId}")]
    public Task CreateOrderDeliveriesAsync(Guid orderId)
    {
        return _ordersAppService.CreateOrderDeliveriesAsync(orderId);
    }

    #endregion
}