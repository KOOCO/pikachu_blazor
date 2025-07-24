using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Users;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Refunds;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.Refund.Default)]
public class RefundAppService : ApplicationService, IRefundAppService
{
    #region Inject
    private readonly IRefundRepository _refundRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
    private readonly IPaymentGatewayAppService _PaymentGatewayAppService;
    private readonly IEmailSender _EmailSender;
    private readonly IEmailAppService _emailAppService;
    GreenWorldLogisticsCreateUpdateDto GreenWorld { get; set; }

    private readonly IConfiguration _Configuration;
    private readonly OrderHistoryManager _orderHistoryManager;
    private readonly IStringLocalizer<PikachuResource> _l;
    private readonly OrderTransactionManager _orderTransactionManager;
    private readonly IUserShoppingCreditRepository _userShoppingCreditRepository;
    private readonly UserShoppingCreditManager _userShoppingCreditManager;
    private readonly IUserCumulativeCreditRepository _userCumulativeCreditRepository;
    private readonly IUserCumulativeCreditAppService _userCumulativeCreditAppService;
    #endregion

    #region Constructor
    public RefundAppService(
        IRefundRepository refundRepository,
        IOrderRepository orderRepository,
        IRepository<PaymentGateway, Guid> paymentGatewayRepository,
        IEmailSender EmailSender,
        IPaymentGatewayAppService PaymentGatewayAppService,
        IConfiguration Configuration,
        IEmailAppService emailAppService,
        OrderHistoryManager orderHistoryManager, IStringLocalizer<PikachuResource> l,
        OrderTransactionManager orderTransactionManager,
        IUserShoppingCreditRepository userShoppingCreditRepository,
        UserShoppingCreditManager userShoppingCreditManager,
        IUserCumulativeCreditRepository userCumulativeCreditRepository,
        IUserCumulativeCreditAppService userCumulativeCreditAppService
    )
    {
        _refundRepository = refundRepository;
        _orderRepository = orderRepository;
        _paymentGatewayRepository = paymentGatewayRepository;
        _EmailSender = EmailSender;
        GreenWorld = new();
        _PaymentGatewayAppService = PaymentGatewayAppService;

        _Configuration = Configuration;
        _emailAppService = emailAppService;
        _l = l;
        _orderHistoryManager = orderHistoryManager;
        _orderTransactionManager = orderTransactionManager;
        _userShoppingCreditRepository = userShoppingCreditRepository;
        _userShoppingCreditManager = userShoppingCreditManager;
        _userCumulativeCreditRepository = userCumulativeCreditRepository;
        _userCumulativeCreditAppService = userCumulativeCreditAppService;
    }
    #endregion

    [Authorize(PikachuPermissions.Refund.Create)]
    public async Task CreateAsync(Guid orderId)
    {
        var existing = await _refundRepository.FindAsync(x => x.OrderId == orderId);
        if (existing != null)
        {
            throw new BusinessException(PikachuDomainErrorCodes.RefundForSameOrderAlreadyExists);
        }
        var refund = new Refund(GuidGenerator.Create(), orderId);
        await _refundRepository.InsertAsync(refund);
        var order = await _orderRepository.GetAsync(orderId);
        order.IsRefunded = true;
        order.OrderRefundType = OrderRefundType.FullRefund;
        // **Get Current User (Editor)**
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var currentUserName = CurrentUser.UserName ?? "System";
        // **Log the creation of the new refunded order**
        await _orderHistoryManager.AddOrderHistoryAsync(
   order.Id,
   "OrderApplyForRefund", // Localization key
   new object[] { order.OrderNo }, // Dynamic placeholders
   currentUserId,
   currentUserName
);
        await _orderRepository.UpdateAsync(order);

        /// ToDo: Send Refund Email Here, and also change status for order
    }

    public async Task<PagedResultDto<RefundDto>> GetListAsync(GetRefundListDto input)
    {
        if (input.Sorting.IsNullOrEmpty())
        {
            input.Sorting = $"{nameof(Refund.CreationTime)} desc";
        }
        var totalCount = await _refundRepository.GetCountAsync(input.Filter);
        var items = await _refundRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

        return new PagedResultDto<RefundDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Refund>, List<RefundDto>>(items)
        };
    }
    public async Task<long> GetRefundPendingCount()
    {
        return await _refundRepository.GetRundPendingCountAsync();

    }
    public async Task<RefundDto> UpdateRefundReviewAsync(Guid id, RefundReviewStatus input, string? rejectReason = null)
    {
        Refund refund = await _refundRepository.GetAsync(id);

        refund.RefundReview = input;

        if (input is RefundReviewStatus.ReturnedApplication && !string.IsNullOrWhiteSpace(rejectReason))
        {
            refund.RejectReason = rejectReason;
        }

        if (input is RefundReviewStatus.Proccessing ||
            input is RefundReviewStatus.ReturnedApplication)
        {
            refund.ReviewCompletionTime = DateTime.Now;

            refund.Approver = CurrentUser.Name;
        }
        if (input is RefundReviewStatus.Fail || input is RefundReviewStatus.Success)
            refund.Refunder = CurrentUser.Name;

        if (input == RefundReviewStatus.Success)
        {
            var order = await _orderRepository.GetAsync(refund.OrderId);
            if (order.OrderRefundType == OrderRefundType.FullRefund && order.CreditDeductionRecordId.HasValue)
            {
                var userShoppingCredit = await _userShoppingCreditRepository.FirstOrDefaultAsync(x => x.Id == order.CreditDeductionRecordId);
                if (userShoppingCredit != null && order.UserId != null && (!userShoppingCredit.ExpirationDate.HasValue || userShoppingCredit.ExpirationDate > DateTime.Now))
                {
                    await _userShoppingCreditManager.CreateAsync(order.UserId.Value, order.CreditDeductionAmount, order.CreditDeductionAmount,
                        $"訂單取消 #{order.OrderNo}.", userShoppingCredit.ExpirationDate, userShoppingCredit.IsActive, UserShoppingCreditType.Grant,
                        order.OrderNo);

                    var userCumulativeCredit = await _userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == order.UserId);
                    if (userCumulativeCredit is null)
                    {
                        await _userCumulativeCreditAppService.CreateAsync(new CreateUserCumulativeCreditDto { TotalAmount = order.CreditDeductionAmount, TotalDeductions = 0, TotalRefunds = 0, UserId = order.UserId.Value });
                    }
                    else
                    {
                        userCumulativeCredit.ChangeTotalAmount((int)(userCumulativeCredit.TotalAmount + order.CreditDeductionAmount));
                        await _userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
                    }
                }
            }
        }

        await _refundRepository.UpdateAsync(refund);

        await _refundRepository.EnsurePropertyLoadedAsync(refund, r => r.Order);

        if (input is RefundReviewStatus.Success)
        {
            await _emailAppService.SendRefundEmailAsync(refund.OrderId, (double)refund.Order.TotalAmount);
        }

        return ObjectMapper.Map<Refund, RefundDto>(refund);
    }

    public async Task CheckStatusAndRequestRefundAsync(Guid id)
    {
        Refund refund = await _refundRepository.GetAsync(id);

        Order order = await _orderRepository.GetWithDetailsAsync(refund.OrderId);
        if (order.PaymentMethod == PaymentMethods.ManualBankTransfer)
        {
            await UpdateRefundReviewAsync(id, RefundReviewStatus.Success);
            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                order.TotalAmount, TransactionType.Refund, TransactionStatus.Successful, PaymentChannel.CashOnDelivery);
            await _orderTransactionManager.CreateAsync(orderTransaction);
            return;

        }

        PaymentGatewayDto? ecpay = (await _PaymentGatewayAppService.GetAllAsync()).FirstOrDefault(f => f.PaymentIntegrationType is PaymentIntegrationType.EcPay) ??
                                    throw new UserFriendlyException("Please Set Ecpay Setting First"); ;

        string status = await GetPaymentStatus(order, ecpay);

        if (status.IsNullOrEmpty())
        {
            await UpdateRefundReviewAsync(id, RefundReviewStatus.Fail);
            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                order.TotalAmount, TransactionType.Refund, TransactionStatus.Failed, PaymentChannel.EcPay);
            await _orderTransactionManager.CreateAsync(orderTransaction);
            return;
        }

        if (order.OrderRefundType is not null && order.OrderRefundType is OrderRefundType.PartialRefund)
        {
            if (status is "已授權")
            {
                await SendRefundRequestAsync(refund, order, ecpay, "C");

                await SendRefundRequestAsync(refund, order, ecpay, "R", true);
            }

            else if (status is "已關帳" || status is "要關帳") await SendRefundRequestAsync(refund, order, ecpay, "R", true);
        }

        else
        {
            if (status is "已授權") await SendRefundRequestAsync(refund, order, ecpay, "N", true);

            else if (status is "已關帳") await SendRefundRequestAsync(refund, order, ecpay, "R", true);

            else if (status is "要關帳")
            {
                await SendRefundRequestAsync(refund, order, ecpay, "E");

                await SendRefundRequestAsync(refund, order, ecpay, "N", true);
            }
        }

        RefundDto refundDto = await UpdateRefundReviewAsync(id, refund.RefundReview);

        if (refundDto.RefundReview is RefundReviewStatus.Success)
        {
            Order OriginalOrder = new();

            if (order.SplitFromId is null || order.SplitFromId == Guid.Empty) OriginalOrder = order;

            else OriginalOrder = await _orderRepository.GetWithDetailsAsync((Guid)order.SplitFromId);

            //OriginalOrder.TotalAmount -= order.TotalAmount;

            if (
                order.ShippingStatus is ShippingStatus.WaitingForPayment ||
                order.ShippingStatus is ShippingStatus.PrepareShipment ||
                order.ShippingStatus is ShippingStatus.ToBeShipped ||
                order.ShippingStatus is ShippingStatus.EnterpricePurchase
            )
                OriginalOrder.TotalAmount -= order.TotalAmount;

            else if (
                order.ShippingStatus is ShippingStatus.Shipped ||
                order.ShippingStatus is ShippingStatus.Delivered ||
                order.ShippingStatus is ShippingStatus.Completed ||
                order.ShippingStatus is ShippingStatus.Return ||
                order.ShippingStatus is ShippingStatus.Closed
            )
                OriginalOrder.TotalAmount -= order.TotalAmount - (order.DeliveryCost ?? 0);

            //OriginalOrder.TotalQuantity -= order.TotalQuantity;

            foreach (OrderItem orderItem in OriginalOrder.OrderItems)
            {
                if (!OriginalOrder.ReturnedOrderItemIds.IsNullOrEmpty())
                {
                    List<Guid> returnedOrderItemGuids = [];

                    List<string>? returnedOrderItemIds = [.. OriginalOrder.ReturnedOrderItemIds.Split(',')];

                    if (returnedOrderItemIds is { Count: > 0 })
                    {
                        foreach (string item in returnedOrderItemIds)
                        {
                            returnedOrderItemGuids.Add(Guid.Parse(item));
                        }

                        if (returnedOrderItemGuids.Any(a => a == orderItem.Id))
                        {
                            orderItem.TotalAmount -= orderItem.TotalAmount;

                            orderItem.ItemPrice -= orderItem.ItemPrice;

                            orderItem.Quantity -= orderItem.Quantity;
                        }
                    }
                }
            }

            //await _orderRepository.UpdateAsync(order);

            await _orderRepository.UpdateAsync(OriginalOrder);
        }
    }

    private async Task<string> GetPaymentStatus(Order order, PaymentGatewayDto? ecpay)
    {
        if (order.PaymentMethod is PaymentMethods.CreditCard)
        {
            List<PaymentGatewayDto> paymentGateways = await _PaymentGatewayAppService.GetAllAsync();

            if (ecpay is null) return string.Empty;

            RestClientOptions options = new() { MaxTimeout = -1 };

            RestClient client = new(options);

            RestRequest request = new(_Configuration["EcPay:SingleCreditCardTransactionApi"], Method.Post);

            string HashKey = ecpay.HashKey ?? string.Empty;

            string HashIV = ecpay.HashIV ?? string.Empty;

            string MerchantId = ecpay.MerchantId ?? string.Empty;

            string totalAmount = order.TotalAmount.ToString(order.TotalAmount % 1 is 0 ? "0" : string.Empty);

            string creditCheckCode = await _PaymentGatewayAppService.GetCreditCheckCodeAsync() ?? string.Empty;

            request.AddHeader("Accept", "text/html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("MerchantID", MerchantId);
            request.AddParameter("CreditRefundId", (order.GWSR ?? 0).ToString());
            request.AddParameter("CreditAmount", totalAmount);
            request.AddParameter("CreditCheckCode", creditCheckCode);
            request.AddParameter("CheckMacValue", GenerateCheckMac(HashKey,
                                                                   HashIV,
                                                                   MerchantId,
                                                                   order.GWSR ?? 0,
                                                                   totalAmount,
                                                                   creditCheckCode));

            RestResponse response = await client.ExecuteAsync(request);

            PaymentStatus? paymentStatus = System.Text.Json.JsonSerializer.Deserialize<PaymentStatus>(response.Content);

            if (paymentStatus is null || paymentStatus.RtnValue?.status is null) return string.Empty;

            return paymentStatus.RtnValue?.status ?? string.Empty;
        }

        return string.Empty;
    }

    public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, int gwsr, string totalAmount, string creditCheckCode)
    {
        Dictionary<string, string> parameters = new()
        {
            { "MerchantID", merchantID },
            { "CreditRefundId", gwsr.ToString() },
            { "CreditAmount", totalAmount },
            { "CreditCheckCode", creditCheckCode }
        };

        IEnumerable<string>? param = parameters.ToDictionary().Keys
                                      .OrderBy(o => o)
                                      .Select(s => s + "=" + parameters.ToDictionary()[s]);

        string collectionValue = string.Join("&", param);

        collectionValue = $"HashKey={HashKey}" + "&" + collectionValue + $"&HashIV={HashIV}";

        collectionValue = WebUtility.UrlEncode(collectionValue).ToLower();

        return ComputeSHA256Hash(collectionValue);
    }

    public async Task SendRefundRequestAsync(Refund refund, Order order, PaymentGatewayDto? ecpay, string action, bool sendEmail = false)
    {
        string logisticSubType = string.Empty; int refundAmount = 0;

        if (
            order.ShippingStatus is ShippingStatus.WaitingForPayment ||
            order.ShippingStatus is ShippingStatus.PrepareShipment ||
            order.ShippingStatus is ShippingStatus.ToBeShipped ||
            order.ShippingStatus is ShippingStatus.EnterpricePurchase
        )
            refundAmount = (int)order.TotalAmount;

        else if (
            order.ShippingStatus is ShippingStatus.Shipped ||
            order.ShippingStatus is ShippingStatus.Delivered ||
            order.ShippingStatus is ShippingStatus.Completed ||
            order.ShippingStatus is ShippingStatus.Return ||
            order.ShippingStatus is ShippingStatus.Closed
        )
            refundAmount = (int)order.TotalAmount - ((int?)order.DeliveryCost ?? 0);

        RestClientOptions options = new() { MaxTimeout = -1 };

        RestClient client = new(options);

        RestRequest request = new("https://payment.ecpay.com.tw/CreditDetail/DoAction", Method.Post);

        string HashKey = ecpay.HashKey ?? string.Empty;
        string HashIV = ecpay.HashIV ?? string.Empty;
        string MerchantId = ecpay.MerchantId ?? string.Empty;
        request.AddHeader("Accept", "text/html");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("MerchantID", MerchantId);
        request.AddParameter("MerchantTradeNo", (refund.Order?.MerchantTradeNo) ?? (refund.Order?.OrderNo));
        request.AddParameter("TradeNo", refund.Order?.TradeNo);
        request.AddParameter("TotalAmount", refundAmount);
        request.AddParameter("Action", action);
        request.AddParameter("CheckMacValue", GenerateCheckMac(HashKey,
                                                               HashIV,
                                                               MerchantId,
                                                               refund.Order?.MerchantTradeNo ?? refund.Order?.OrderNo,
                                                               refund.Order?.TradeNo,
                                                               action,
                                                               refundAmount.ToString()));

        RestResponse response = await client.ExecuteAsync(request);

        NameValueCollection queryParams = HttpUtility.ParseQueryString(response.Content);

        RefundResponse result = new()
        {
            MerchantID = queryParams["MerchantID"] ?? string.Empty,
            MerchantTradeNo = queryParams["MerchantTradeNo"] ?? string.Empty,
            TradeNo = queryParams["TradeNo"] ?? string.Empty,
            RtnMsg = queryParams["RtnMsg"] ?? string.Empty,
            RtnCode = int.Parse(queryParams["RtnCode"])
        };

        var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                refundAmount, TransactionType.Refund, TransactionStatus.Successful, PaymentChannel.EcPay);

        if (result is not null && sendEmail)
        {
            if (result.RtnCode is 1)
            {
                refund.RefundReview = RefundReviewStatus.Success;

                //if (!order.CustomerEmail.IsNullOrEmpty())
                //await _emailAppService.SendRefundEmailAsync(order.Id, refundAmount);
            }

            else
            {
                refund.RefundReview = RefundReviewStatus.Fail;

                orderTransaction.TransactionStatus = TransactionStatus.Failed;
                orderTransaction.FailedReason = result.RtnMsg;
            }

            await _orderTransactionManager.CreateAsync(orderTransaction);
        }
    }

    public async Task SendRefundRequestAsync(Guid id)
    {
        Refund refund = await _refundRepository.GetAsync(id);

        Order order = await _orderRepository.GetAsync(refund.OrderId);

        PaymentGatewayDto? ecpay = (await _PaymentGatewayAppService.GetAllAsync()).FirstOrDefault(f => f.PaymentIntegrationType is PaymentIntegrationType.EcPay) ??
                                    throw new UserFriendlyException("Please Set Ecpay Setting First"); ;

        //PaymentGateway? ecpay = (await _paymentGatewayRepository.GetQueryableAsync()).FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay) ?? 
        //                        throw new UserFriendlyException("Please Set Ecpay Setting First");

        string logisticSubType = string.Empty;

        RestClientOptions options = new() { MaxTimeout = -1 };

        RestClient client = new(options);

        RestRequest request = new("https://payment.ecpay.com.tw/CreditDetail/DoAction", Method.Post);

        string HashKey = ecpay.HashKey ?? string.Empty;
        string HashIV = ecpay.HashIV ?? string.Empty;
        string MerchantId = ecpay.MerchantId ?? string.Empty;
        request.AddHeader("Accept", "text/html");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("MerchantID", MerchantId);
        request.AddParameter("MerchantTradeNo", (refund.Order?.MerchantTradeNo) ?? (refund.Order?.OrderNo));
        request.AddParameter("TradeNo", refund.Order?.TradeNo);
        request.AddParameter("TotalAmount", (int)(refund.Order?.TotalAmount));
        request.AddParameter("Action", "R");
        request.AddParameter("CheckMacValue", GenerateCheckMac(HashKey,
                                                               HashIV,
                                                               MerchantId,
                                                               refund.Order?.MerchantTradeNo ?? refund.Order?.OrderNo,
                                                               refund.Order?.TradeNo,
                                                               "R",
                                                               ((int)refund.Order?.TotalAmount).ToString()));
        //request.AddParameter("IsCollection", "N");

        RestResponse response = await client.ExecuteAsync(request);

        NameValueCollection queryParams = HttpUtility.ParseQueryString(response.Content);

        RefundResponse result = new()
        {
            MerchantID = queryParams["MerchantID"] ?? string.Empty,
            MerchantTradeNo = queryParams["MerchantTradeNo"] ?? string.Empty,
            TradeNo = queryParams["TradeNo"] ?? string.Empty,
            RtnMsg = queryParams["RtnMsg"] ?? string.Empty,
            RtnCode = int.Parse(queryParams["RtnCode"])
        };

        var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                (int)refund.Order?.TotalAmount, TransactionType.Refund, TransactionStatus.Successful, PaymentChannel.EcPay);

        if (result is not null)
        {
            if (result.RtnCode is 1)
            {
                refund.RefundReview = RefundReviewStatus.Success;

                //if (!order.CustomerEmail.IsNullOrEmpty())
                //    await _emailAppService.SendRefundEmailAsync(order.Id, (double?)refund.Order?.TotalAmount ?? 0);
            }

            else
            {
                refund.RefundReview = RefundReviewStatus.Fail;

                orderTransaction.TransactionStatus = TransactionStatus.Failed;
                orderTransaction.FailedReason = result.RtnMsg;
            }

            await _orderTransactionManager.CreateAsync(orderTransaction);
        }
    }
    public string GenerateCheckMac(string HashKey, string HashIV, string merchantID, string merchantTradeNo, string tradeNo, string action, string totalamount)
    {
        var parameters = new Dictionary<string, string>
        {
            { "MerchantID", merchantID },
            { "MerchantTradeNo", merchantTradeNo },
            { "TradeNo", tradeNo },
            { "Action", action },
            { "TotalAmount", totalamount }
        };

        IEnumerable<string>? param = parameters.ToDictionary().Keys
                                      .OrderBy(o => o)
                                      .Select(s => s + "=" + parameters.ToDictionary()[s]);

        string collectionValue = string.Join("&", param);

        collectionValue = $"HashKey={HashKey}" + "&" + collectionValue + $"&HashIV={HashIV}";

        collectionValue = WebUtility.UrlEncode(collectionValue).ToLower();

        return ComputeSHA256Hash(collectionValue);
    }
    public string ComputeSHA256Hash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

        StringBuilder builder = new();

        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }

        return builder.ToString().ToUpper();
    }
}
public class RefundResponse
{

    public string MerchantID { get; set; }
    public string MerchantTradeNo { get; set; }
    public string TradeNo { get; set; }
    public string RtnMsg { get; set; }
    public int RtnCode { get; set; }

}

public class PaymentStatus
{
    public string? RtnMsg { get; set; }
    public RtnValue? RtnValue { get; set; }
}

public class RtnValue
{
    public string? TradeID { get; set; }
    public string? amount { get; set; }
    public string? clsamt { get; set; }
    public string? authtime { get; set; }
    public string? status { get; set; }
    public CloseData[]? close_data { get; set; }
}

public class CloseData
{
    public string? status { get; set; }
    public string? sno { get; set; }
    public string? amount { get; set; }
    public string? datetime { get; set; }
}