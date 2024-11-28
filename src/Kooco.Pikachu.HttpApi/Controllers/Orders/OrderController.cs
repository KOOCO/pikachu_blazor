using Asp.Versioning;
using ECPay.Payment.Integration;
using Kooco.Pikachu.Controllers.Common;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.StoreLogisticOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
public class OrderController : AbpController, IOrderAppService
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

    [HttpGet("ecpay-proceed-to-checkout")]
    public async Task<IActionResult> ProceedToCheckout(Guid orderId, string clientBackUrl)
    {
        OrderDto order = await _ordersAppService.GetWithDetailsAsync(orderId);

        GroupBuyDto groupBuy = await _GroupBuyAppService.GetAsync(order.GroupBuyId);

        PaymentGatewayDto ecPay = await GetPaymentGatewayConfigurationsAsync(order.GroupBuyId);

        bool isDefaultPayment = groupBuy.IsDefaultPaymentGateWay;

        string? paymentMethods = groupBuy.PaymentMethod;

        PaymentMethod paymentMethod = new();

        if (!string.IsNullOrEmpty(paymentMethods))
        {
            if (order.PaymentMethod is PaymentMethods.BankTransfer)
            {
                paymentMethod = PaymentMethod.WebATM;

                isDefaultPayment = false;
            }
            else if (order.PaymentMethod is PaymentMethods.CreditCard)
            {
                paymentMethod = PaymentMethod.Credit;

                isDefaultPayment = false;
            }
        }

        else isDefaultPayment = true;

        string ecpayUrl = _Configuration["EcPay:PaymentApiUrl"]!;
        string? hashKey = ecPay.HashKey;
        string? hashIV = ecPay.HashIV;
        string? merchantID = ecPay.MerchantId;
        string? tradeDesc = ecPay.TradeDescription;

        List<string> enErrors = [];

        try
        {
            if (order.DeliveryMethod is DeliveryMethod.SelfPickup || groupBuy.IsEnterprise)
            {
                if (order.OrderItems.Any(x => x.DeliveryTemperature == 0))
                    order.TotalAmount = order.TotalAmount - order.OrderItems.Where(x => x.DeliveryTemperature == 0)
                                                                            .Select(x => x.DeliveryTemperatureCost)
                                                                            .FirstOrDefault();

                if (order.OrderItems.Any(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze))
                    order.TotalAmount = order.TotalAmount - order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze)
                                                                            .Select(x => x.DeliveryTemperatureCost)
                                                                            .FirstOrDefault();

                if (order.OrderItems.Any(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen))
                    order.TotalAmount = order.TotalAmount - order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen)
                                                                            .Select(x => x.DeliveryTemperatureCost)
                                                                            .FirstOrDefault();
            }

            using AllInOne oPayment = new();

            oPayment.ServiceMethod = HttpMethod.HttpPOST;
            oPayment.ServiceURL = ecpayUrl;
            oPayment.HashKey = hashKey;
            oPayment.HashIV = hashIV;
            oPayment.MerchantID = merchantID;
            oPayment.Send.ReturnURL = $"{Request.Scheme}://{Request.Host}/api/app/orders/callback";
            oPayment.Send.ClientBackURL = string.Empty;
            oPayment.Send.MerchantTradeNo = order.OrderNo;
            oPayment.Send.MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            oPayment.Send.TotalAmount = Convert.ToInt32(order.TotalAmount);
            oPayment.Send.TradeDesc = tradeDesc;
            oPayment.Send.ChoosePayment = isDefaultPayment ? PaymentMethod.ALL : paymentMethod;
            oPayment.Send.Remark = string.Empty;
            oPayment.Send.ChooseSubPayment = PaymentMethodItem.None;
            oPayment.Send.NeedExtraPaidInfo = ExtraPaymentInfo.Yes;
            oPayment.Send.DeviceSource = DeviceType.PC;
            oPayment.Send.IgnorePayment = string.Empty;
            oPayment.Send.PlatformID = string.Empty;
            oPayment.Send.HoldTradeAMT = HoldTradeType.Yes;
            oPayment.Send.CustomField1 = Guid.NewGuid().ToString();
            oPayment.Send.CustomField2 = order.GroupBuyId.ToString();
            oPayment.Send.CustomField3 = string.Empty;
            oPayment.Send.CustomField4 = string.Empty;
            oPayment.Send.EncryptType = 1;

            foreach (OrderItemDto item in order.OrderItems)
            {
                if (item.Item is null) continue;

                oPayment.Send.Items.Add(new ECPay.Payment.Integration.Item()
                {
                    Name = item.Item.ItemName + " NT$ ",
                    Price = decimal.TryParse(item.ItemPrice.ToString("G29"), out decimal price) ? price : 0.00M,
                    Currency = string.Empty,
                    Quantity = item.Quantity,
                    URL = string.Empty,
                });
            }

            AllInOneResult result = oPayment.CheckOut();

            enErrors = [.. result.ErrorList];

            PaymentResponse? response =
                new()
                {
                    IsSuccessStatusCode = true,
                    Result = result,
                    Errors = enErrors,
                    EcPayUrl = ecpayUrl
                };

            if (response.IsSuccessStatusCode)
            {
                string? checkMacValue = response.Result.htParameters["CustomField1"]?.ToString();

                await _ordersAppService.AddValuesAsync(orderId, checkMacValue ?? string.Empty, order.OrderNo);
            }

            StringBuilder htmlForm = new();

            htmlForm.Append($"<form id='ecpay_form' method='post' action='{response.EcPayUrl}'>");

            foreach (DictionaryEntry parameter in response.Result.htParameters)
            {
                htmlForm.Append(
                    $"<input type='hidden' name='{parameter.Key}' value='{parameter.Value}'>"
                );
            }

            htmlForm.Append("</form>");

            htmlForm.Append("<script>document.getElementById('ecpay_form').submit();</script>");

            return Ok(htmlForm.ToString());
        }
        catch (Exception ex)
        {
            enErrors.Add(ex.Message);

            return BadRequest(
                new PaymentResponse { IsSuccessStatusCode = false, Errors = enErrors }
            );
        }
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

            await _ordersAppService.UpdateLogisticStatusAsync(ecpayRequest.MerchantTradeNo!, ecpayRequest.RtnMsg!);

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
        return _ordersAppService.CreditNoteInvoice(id, reason);
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
    public Task UpdateLogisticStatusAsync(string merchantTradeNo, string rtnMsg)
    {
        return _ordersAppService.UpdateLogisticStatusAsync(merchantTradeNo, rtnMsg);
    }

    public Task<OrderDto> ChangeOrderStatus(Guid id, ShippingStatus status)
    {
        return _ordersAppService.ChangeOrderStatus(id, status);
    }
    #endregion
}