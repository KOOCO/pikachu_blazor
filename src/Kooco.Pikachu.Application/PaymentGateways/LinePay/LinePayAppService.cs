using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayAppService : PikachuAppService, ILinePayAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGatewayAppService _paymentGatewayAppService;
    private readonly LinePayConfiguration _apiOptions;
    private readonly RestClient _restClient;

    public LinePayAppService(IPaymentGatewayAppService paymentGatewayAppService,
        IOrderRepository orderRepository, IOptions<LinePayConfiguration> linePayConfiguration)
    {
        _orderRepository = orderRepository;
        _paymentGatewayAppService = paymentGatewayAppService;
        _apiOptions = linePayConfiguration.Value;
        _restClient = new RestClient(_apiOptions.ApiBaseUrl);
    }

    public async Task<LinePayResponseDto<LinePayPaymentResponseInfoDto>> PaymentRequest(Guid orderId)
    {
        var linePay = await _paymentGatewayAppService.GetLinePayAsync(true)
            ?? throw new EntityNotFoundException(typeof(PaymentGateway));

        var order = await _orderRepository.GetWithDetailsAsync(orderId)
            ?? throw new EntityNotFoundException(typeof(Order), orderId);

        var body = order.CreatePaymentRequest(_apiOptions, L);

        var nonce = Guid.NewGuid().ToString();

        var signature = LinePayExtensionService.GeneratePostSignature(_apiOptions, linePay.ChannelSecretKey, body, nonce);

        var response = await _restClient.Post(_apiOptions.PaymentApiPath, body, nonce, linePay.ChannelId, signature);

        order.ExtraProperties.TryAdd(PaymentGatewayConsts.PaymentResponse, response.Content);

        var responseDto = LinePayExtensionService.DeserializePaymentRequest(response);

        if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
        {
            order.ShippingStatus = ShippingStatus.WaitingForPayment;
            await _orderRepository.UpdateAsync(order);
            return responseDto;
        }

        order.OrderStatus = OrderStatus.Closed;
        order.ShippingStatus = ShippingStatus.Closed;

        Logger.LogError(@"Error in Line Pay Payment Request: {0}", response.ToString());
        return responseDto;
    }

    public async Task<object> ConfirmPayment(long transactionId, string? orderNo)
    {
        var linePay = await _paymentGatewayAppService.GetLinePayAsync(true)
            ?? throw new EntityNotFoundException(typeof(PaymentGateway));
        var order = await _orderRepository.FirstOrDefaultAsync(x => x.OrderNo == orderNo)
            ?? throw new EntityNotFoundException(typeof(Order), orderNo);

        var body = order.CreateConfirmPaymentRequest();

        var nonce = Guid.NewGuid().ToString();

        var signature = LinePayExtensionService.GeneratePostSignature(_apiOptions, linePay.ChannelSecretKey, body, nonce);

        string apiPath = string.Format(_apiOptions.ConfirmPaymentApiPath, transactionId);

        var response = await _restClient.Post(apiPath, body, nonce, linePay.ChannelId, signature);

        order.ExtraProperties.TryAdd(PaymentGatewayConsts.ConfirmPaymentResponse, response.Content);

        var responseDto = LinePayExtensionService.DeserializePaymentRequest(response);

        if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
        {
            order.ShippingStatus = ShippingStatus.WaitingForPayment;
            await _orderRepository.UpdateAsync(order);
            return responseDto;
        }

        order.OrderStatus = OrderStatus.Closed;
        order.ShippingStatus = ShippingStatus.Closed;

        Logger.LogError(@"Error in Line Pay Payment Request: {0}", response.ToString());
        return responseDto;
    }
}
