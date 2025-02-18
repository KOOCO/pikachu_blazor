using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
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
    private readonly IDataFilter<IMultiTenant> _multiTenantFilter;
    private readonly LinePayConfiguration _apiOptions;
    private readonly RestClient _restClient;

    public LinePayAppService(IOrderRepository orderRepository, IOrderAppService orderAppService,
        IPaymentGatewayAppService paymentGatewayAppService, IDataFilter<IMultiTenant> multiTenantFilter,
        IOptions<LinePayConfiguration> linePayConfiguration)
    {
        _orderRepository = orderRepository;
        _orderAppService = orderAppService;
        _paymentGatewayAppService = paymentGatewayAppService;
        _multiTenantFilter = multiTenantFilter;
        _apiOptions = linePayConfiguration.Value;
        _restClient = new RestClient(_apiOptions.ApiBaseUrl);
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

            order.OrderStatus = OrderStatus.Closed;
            order.ShippingStatus = ShippingStatus.Closed;

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

            var responseDto = LinePayExtensionService.Deserialize<LinePayConfirmResponseInfoDto>(response);

            if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
            {
                order.TradeNo = responseDto.Info?.TransactionId.GetString();
                order.ShippingStatus = ShippingStatus.PrepareShipment;
                order.PrepareShipmentBy = CurrentUser.Name ?? "System";
                order.PaymentDate = DateTime.Now;
                await _orderAppService.CreateOrderDeliveriesAndInvoiceAsync(order.Id);
                return responseDto;
            }

            order.OrderStatus = OrderStatus.Closed;
            order.ShippingStatus = ShippingStatus.Closed;

            Logger.LogError(@"Error in Line Pay Confirm Payment Request: {response}", response.ToString());
            return responseDto;
        }
    }

    public async Task<LinePayResponseDto<LinePayRefundResponseInfoDto>> ProcessRefund(Guid orderId)
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

            var confirmPaymentResponse = order.GetConfirmPaymentResponse()
                        ?? throw new UserFriendlyException("Payment does not exist against this order.");

            var body = JsonSerializer.Serialize(new
            {
                refundAmount = (int)order.TotalAmount
            });

            var nonce = Guid.NewGuid().ToString();

            var apiPath = string.Format(_apiOptions.RefundApiPath, confirmPaymentResponse.Info.TransactionId);

            var signature = LinePayExtensionService.GeneratePostSignature(apiPath, linePay.ChannelSecretKey, body, nonce);

            var response = await _restClient.Post(apiPath, body, nonce, linePay.ChannelId, signature);

            order.ExtraProperties.TryAdd(PaymentGatewayConsts.RefundResponse, response.Content);

            var responseDto = LinePayExtensionService.Deserialize<LinePayRefundResponseInfoDto>(response);

            if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
            {
                order.RefundAmount = order.TotalAmount;
                order.IsRefunded = true;
                order.OrderStatus = OrderStatus.Refund;
                order.OrderRefundType = OrderRefundType.FullRefund;
            }
            else
            {
                Logger.LogError(@"Error in Line Pay Refund Request: {response}", response.ToString());
            }
            return responseDto;
        }
    }
}
