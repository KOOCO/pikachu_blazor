using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.Refunds;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

[RemoteService(IsEnabled = false)]
public class LinePayAppService : PikachuAppService, ILinePayAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderAppService _orderAppService;
    private readonly IPaymentGatewayAppService _paymentGatewayAppService;
    private readonly IRefundRepository _refundRepository;
    private readonly IDataFilter<IMultiTenant> _multiTenantFilter;
    private readonly IEmailAppService _emailAppService;
    private readonly IRefundAppService _refundAppService;
    private readonly LinePayConfiguration _apiOptions;
    private readonly RestClient _restClient;
    private readonly OrderHistoryManager _orderHistoryManager;
    private readonly IStringLocalizer<PikachuResource> _l;
    private readonly OrderTransactionManager _orderTransactionManager;


    public LinePayAppService(IOrderRepository orderRepository, IOrderAppService orderAppService,
        IPaymentGatewayAppService paymentGatewayAppService, IDataFilter<IMultiTenant> multiTenantFilter,
        IEmailAppService emailAppService, IRefundAppService refundAppService,
        IRefundRepository refundRepository, IOptions<LinePayConfiguration> linePayConfiguration,
        OrderHistoryManager orderHistoryManager, IStringLocalizer<PikachuResource> l, OrderTransactionManager orderTransactionManager)
    {
        _orderRepository = orderRepository;
        _orderAppService = orderAppService;
        _paymentGatewayAppService = paymentGatewayAppService;
        _refundRepository = refundRepository;
        _multiTenantFilter = multiTenantFilter;
        _emailAppService = emailAppService;
        _refundAppService = refundAppService;
        _apiOptions = linePayConfiguration.Value;
        _restClient = new RestClient(_apiOptions.ApiBaseUrl);
        _orderHistoryManager = orderHistoryManager;
        _l = l;
        _orderTransactionManager = orderTransactionManager;
    }

    public async Task<LinePayResponseDto<LinePayPaymentResponseInfoDto>> PaymentRequest(Guid orderId, LinePayPaymentRequestRedirectUrlDto input)
    {
        Order order;
        using (_multiTenantFilter.Disable())
        {
            order = await _orderRepository.GetAsync(orderId);
        }

        using (CurrentTenant.Change(order.TenantId))
        {
            var linePay = await _paymentGatewayAppService.GetLinePayAsync(true)
                ?? throw new EntityNotFoundException(typeof(PaymentGateway));

            await _orderRepository.EnsurePropertyLoadedAsync(order, o => o.GroupBuy);

            var body = order.CreatePaymentRequest(L, linePay.LinePointsRedemption, input);

            var nonce = Guid.NewGuid().ToString();

            var signature = LinePayExtensionService.GeneratePostSignature(_apiOptions.PaymentApiPath, linePay.ChannelSecretKey, body, nonce);

            var response = await _restClient.Post(_apiOptions.PaymentApiPath, body, nonce, linePay.ChannelId, signature);

            order.ExtraProperties.TryAdd(PaymentGatewayConsts.PaymentResponse, response.Content);

            var responseDto = LinePayExtensionService.Deserialize<LinePayPaymentResponseInfoDto>(response);

            if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
            {
                order.ShippingStatus = ShippingStatus.WaitingForPayment;
                await _orderRepository.UpdateAsync(order);
                return responseDto;
            }
            if (order.TotalAmount != 0)
            {
                var oldShippingStatus = order.ShippingStatus;
                order.OrderStatus = OrderStatus.Closed;
                order.ShippingStatus = ShippingStatus.Closed;
                // **Get Current User (Editor)**
                var currentUserId = CurrentUser.Id ?? Guid.Empty;
                var currentUserName = CurrentUser.UserName ?? "System";

                // **Log Order History for Order Closure**
                await _orderHistoryManager.AddOrderHistoryAsync(
             order.Id,
             "OrderClosed", // Localization key
             new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name }, // Localized placeholders
             currentUserId,
             currentUserName
         );
               
            }
            Logger.LogError(@"Error in Line Pay Payment Request: {response}", response.ToString());
            return responseDto;
        }
    }

    public async Task<LinePayResponseDto<LinePayConfirmResponseInfoDto>> ConfirmPayment(string transactionId, string? orderNo)
    {
        Order order;
        using (_multiTenantFilter.Disable())
        {
            order = await _orderRepository.FirstOrDefaultAsync(x => x.OrderNo == orderNo)
                        ?? throw new EntityNotFoundException(typeof(Order), orderNo);
        }

        using (CurrentTenant.Change(order.TenantId))
        {
            var linePay = await _paymentGatewayAppService.GetLinePayAsync(true)
                ?? throw new EntityNotFoundException(typeof(PaymentGateway));

            var body = order.CreateConfirmPaymentRequest();

            var nonce = Guid.NewGuid().ToString();

            string apiPath = string.Format(_apiOptions.ConfirmPaymentApiPath, transactionId);

            var signature = LinePayExtensionService.GeneratePostSignature(apiPath, linePay.ChannelSecretKey, body, nonce);

            var response = await _restClient.Post(apiPath, body, nonce, linePay.ChannelId, signature);

            order.ExtraProperties.TryAdd(PaymentGatewayConsts.ConfirmPaymentResponse, response.Content);
            order.PaymentDate = DateTime.Now;

            var responseDto = LinePayExtensionService.Deserialize<LinePayConfirmResponseInfoDto>(response);
            var oldShippingStatus = order.ShippingStatus;
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                order.TotalAmount, TransactionType.Payment, TransactionStatus.Successful, PaymentChannel.LinePay);

            if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
            {
                order.TradeNo = responseDto.Info?.TransactionId.ToString();

                order.ShippingStatus = ShippingStatus.PrepareShipment;
                order.PrepareShipmentBy = CurrentUser.Name ?? "System";

                await _orderAppService.CreateOrderDeliveriesAndInvoiceAsync(order.Id);
                return responseDto;
            }

            else
            {
                order.OrderStatus = OrderStatus.Closed;
                order.ShippingStatus = ShippingStatus.Closed;

                orderTransaction.TransactionStatus = TransactionStatus.Failed;
                orderTransaction.FailedReason = responseDto.ReturnMessage;

                Logger.LogError(@"Error in Line Pay Confirm Payment Request: {response}", response.ToString());
            }

            await _orderTransactionManager.CreateAsync(orderTransaction);

            // **Log Order History for Payment Processing**
            await _orderHistoryManager.AddOrderHistoryAsync(
                  order.Id,
                  "PaymentProcessed",
                  new object[] { _l[oldShippingStatus.ToString()].Name, _l[order.ShippingStatus.ToString()].Name },
                  currentUserId,
                  currentUserName
                );

            return responseDto;
        }
    }

    public async Task<LinePayResponseDto<LinePayRefundResponseInfoDto>> ProcessRefund(Guid refundId)
    {
        Refund refund;
        using (_multiTenantFilter.Disable())
        {
            refund = await _refundRepository.GetAsync(refundId);
        }

        using (CurrentTenant.Change(refund.TenantId))
        {
            var order = await _orderRepository.GetAsync(refund.OrderId);

            var linePay = await _paymentGatewayAppService.GetLinePayAsync(true)
                        ?? throw new EntityNotFoundException(typeof(PaymentGateway));

            var confirmPaymentResponse = order.GetConfirmPaymentResponse()
                        ?? throw new UserFriendlyException("Payment does not exist against this order.");

            int deliveryCost = order.ShippingStatus switch
            {
                ShippingStatus.Shipped or
                ShippingStatus.Delivered or
                ShippingStatus.Completed or
                ShippingStatus.Return or
                ShippingStatus.Closed => (int)(order.DeliveryCost ?? 0),
                _ => 0
            };

            int refundAmount = (int)(order.TotalAmount - deliveryCost);

            var body = JsonSerializer.Serialize(new
            {
                refundAmount = refundAmount
            });

            var nonce = Guid.NewGuid().ToString();

            var apiPath = string.Format(_apiOptions.RefundApiPath, confirmPaymentResponse.Info.TransactionId);

            var signature = LinePayExtensionService.GeneratePostSignature(apiPath, linePay.ChannelSecretKey, body, nonce);

            var response = await _restClient.Post(apiPath, body, nonce, linePay.ChannelId, signature);

            order.ExtraProperties.TryAdd(PaymentGatewayConsts.RefundResponse, response.Content);

            var responseDto = LinePayExtensionService.Deserialize<LinePayRefundResponseInfoDto>(response);

            var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                refundAmount, TransactionType.Refund, TransactionStatus.Successful, PaymentChannel.LinePay);

            if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
            {
                RefundDto refundDto = await _refundAppService.UpdateRefundReviewAsync(refundId, RefundReviewStatus.Success);

                if (refundDto.RefundReview is RefundReviewStatus.Success)
                {
                    Order OriginalOrder = new();

                    if (order.SplitFromId is null || order.SplitFromId == Guid.Empty) OriginalOrder = order;

                    else OriginalOrder = await _orderRepository.GetWithDetailsAsync((Guid)order.SplitFromId);

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

                    await _orderRepository.UpdateAsync(OriginalOrder);
                }
            }
            else
            {
                orderTransaction.TransactionStatus = TransactionStatus.Failed;
                orderTransaction.FailedReason = responseDto.ReturnMessage;

                Logger.LogError(@"Error in Line Pay Refund Request: {response}", response.ToString());
            }

            await _orderTransactionManager.CreateAsync(orderTransaction);
            return responseDto;
        }
    }
}
