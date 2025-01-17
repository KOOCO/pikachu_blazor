using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayAppService(IPaymentGatewayAppService paymentGatewayAppService,
        IOrderRepository orderRepository, IOptions<LinePayConfiguration> linePayConfiguration) : PikachuAppService, ILinePayAppService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPaymentGatewayAppService _paymentGatewayAppService = paymentGatewayAppService;
    private readonly LinePayConfiguration _apiOptions = linePayConfiguration.Value;

    public async Task<LinePayPaymentResponseDto> PaymentRequest(Guid orderId)
    {
        var linePay = await _paymentGatewayAppService.GetLinePayAsync(true)
            ?? throw new EntityNotFoundException(typeof(PaymentGateway));

        var order = await _orderRepository.GetWithDetailsAsync(orderId)
            ?? throw new EntityNotFoundException(typeof(Order), orderId);

        var body = CreateLinePayRequest(order);

        var serializedBody = JsonSerializer.Serialize(body, GetJsonOptions());
        var nonce = Guid.NewGuid().ToString();

        var signature = GenerateHmacSignature(linePay.ChannelSecretKey, GeneratePostSignatureBase(linePay.ChannelSecretKey, serializedBody, nonce));

        var response = await SendLinePayRequest(serializedBody, nonce, linePay.ChannelId, signature);

        order.ExtraProperties.TryAdd("PaymentResponse", response.Content);

        var responseDto = response.Content == null ? new()
            : JsonSerializer.Deserialize<LinePayPaymentResponseDto>(response.Content, GetJsonOptions());

        responseDto ??= new();

        if (response.IsSuccessful && responseDto.ReturnCode == _apiOptions.SuccessReturnCode)
        {
            order.ShippingStatus = ShippingStatus.WaitingForPayment;
            await _orderRepository.UpdateAsync(order);
            return responseDto;
        }

        order.OrderStatus = OrderStatus.Closed;
        order.ShippingStatus = ShippingStatus.Closed;

        Logger.LogError($"Line Pay response exception: {response.ToString()}");
        return responseDto;
    }

    private LinePayPaymentRequestDto CreateLinePayRequest(Order order)
    {
        var basePackages = order.OrderItems.Select(oi => new LinePayPaymentRequestPackageDto
        {
            Id = oi.Id.ToString(),
            Amount = Convert.ToInt32(oi.TotalAmount),
            Products =
            [
                new() {
                    Id = (oi.ItemId ?? oi.SetItemId).ToString() ?? "",
                    Name = oi.ItemId.HasValue ? oi.Item?.ItemName : oi.SetItem?.SetItemName,
                    ImageUrl = oi.ItemId.HasValue ? oi.Item?.ItemMainImageURL : oi.SetItem?.SetItemMainImageURL,
                    Quantity = oi.Quantity,
                    Price = Convert.ToInt32(oi.ItemPrice)
                }
            ]
        }).ToList();

        var deliveryPackages = CreateDeliveryCostPackages(order);

        return new LinePayPaymentRequestDto
        {
            Amount = (int)order.TotalAmount,
            Currency = "TWD",
            OrderId = order.OrderNo,
            Packages = [.. basePackages, .. deliveryPackages],
            RedirectUrls = new LinePayPaymentRequestRedirectUrlDto
            {
                ConfirmUrl = $"{_apiOptions.SelfUrl}/api/app/line-pay/confirm",
                CancelUrl = $"{_apiOptions.SelfUrl}/api/app/line-pay/cancel"
            }
        };
    }

    private List<LinePayPaymentRequestPackageDto> CreateDeliveryCostPackages(Order order)
    {
        var deliveryCosts = new[]
        {
            new { Label = L["DeliveryCost"], Cost = order.DeliveryCost },
            new { Label = L["DeliveryCostForNormal"], Cost = order.DeliveryCostForNormal },
            new { Label = L["DeliveryCostForFreeze"], Cost = order.DeliveryCostForFreeze },
            new { Label = L["DeliveryCostForFrozen"], Cost = order.DeliveryCostForFrozen }
        };

        var totalCost = deliveryCosts.Sum(d => d.Cost);
        if (totalCost <= 0) return [];

        return
        [
            new LinePayPaymentRequestPackageDto
            {
                Id = Guid.NewGuid().ToString(),
                Amount = (int)totalCost,
                Products = deliveryCosts
                    .Where(d => d.Cost > 0)
                    .Select(d => new LinePayPaymentRequestProductDto
                    {
                        Id = d.Label,
                        Name = d.Label,
                        ImageUrl = string.Empty,
                        Quantity = 1,
                        Price = (int)d.Cost
                    }).ToList()
            }
        ];
    }

    private async Task<RestResponse> SendLinePayRequest(string serializedBody, string nonce, string channelId, string signature)
    {
        var client = new RestClient(_apiOptions.ApiBaseUrl);
        var request = new RestRequest(_apiOptions.PaymentApiPath, Method.Post)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("X-LINE-ChannelId", channelId)
            .AddHeader("X-LINE-Authorization-Nonce", nonce)
            .AddHeader("X-LINE-Authorization", signature)
            .AddStringBody(serializedBody, ContentType.Json);

        return await client.ExecuteAsync(request);
    }

    private string GeneratePostSignatureBase(string channelSecretKey, string serializedBody, string nonce)
    {
        return channelSecretKey + _apiOptions.PaymentApiPath + serializedBody + nonce;
    }

    private static string GenerateHmacSignature(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);

        return Convert.ToBase64String(hash);
    }

    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
