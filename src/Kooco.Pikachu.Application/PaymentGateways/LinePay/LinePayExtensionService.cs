using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Localization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public static class LinePayExtensionService
{
    public static string CreatePaymentRequest(this Order order, IStringLocalizer l, bool linePointsRedemption, LinePayPaymentRequestRedirectUrlDto input)
    {
        var totalAmount = Convert.ToInt32(order.TotalAmount);

        var packages = new List<LinePayPaymentRequestPackageDto>
        {
            new()
            {
                Id = order.OrderNo,
                Amount = totalAmount,
                Name = order.GroupBuy?.GroupBuyName,
                Products =
                [
                    new LinePayPaymentRequestProductDto
                    {
                        Id = "Total",
                        Name = l["Total"],
                        Quantity = 1,
                        Price = totalAmount
                    }
                ]
            }
        };

        var body = new LinePayPaymentRequestDto
        {
            Amount = (int)order.TotalAmount,
            Currency = "TWD",
            OrderId = order.OrderNo,
            Packages = packages,
            RedirectUrls = new LinePayPaymentRequestRedirectUrlDto
            {
                ConfirmUrl = input.ConfirmUrl,
                CancelUrl = input.CancelUrl
            }
        };

        if (!linePointsRedemption)
        {
            body.Options = new()
            {
                Extra = new()
                {
                    PromotionRestriction = new()
                    {
                        UseLimit = 0,
                        RewardLimit = 0
                    }
                }
            };
        }

        return JsonSerializer.Serialize(body, GetJsonOptions());
    }

    public static string CreateConfirmPaymentRequest(this Order order)
    {
        var body = new
        {
            Amount = (int)order.TotalAmount,
            Currency = "TWD"
        };
        return JsonSerializer.Serialize(body, GetJsonOptions());
    }

    public static LinePayResponseDto<TInfo> Deserialize<TInfo>(RestResponse restResponse)
    {
        var responseContent = restResponse.Content;
        return Deserialize<TInfo>(responseContent);
    }

    public static LinePayResponseDto<TInfo> Deserialize<TInfo>(string? responseContent)
    {
        var responseDto = (responseContent == null ? new()
            : JsonSerializer.Deserialize<LinePayResponseDto<TInfo>>(responseContent, GetJsonOptions()))
            ?? new();

        return responseDto;
    }

    public static string GeneratePostSignature(string apiPath, string channelSecretKey, string serializedBody, string nonce)
    {
        var signatureBase = channelSecretKey + apiPath + serializedBody + nonce;
        return GenerateHmacSignature(channelSecretKey, signatureBase);
    }

    public static async Task<RestResponse> Post(this RestClient restClient, string apiPath, string body, string nonce, string channelId, string signature)
    {
        var request = new RestRequest(apiPath, Method.Post)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("X-LINE-ChannelId", channelId)
            .AddHeader("X-LINE-Authorization-Nonce", nonce)
            .AddHeader("X-LINE-Authorization", signature)
            .AddStringBody(body, ContentType.Json);

        return await restClient.ExecuteAsync(request);
    }

    private static string GenerateHmacSignature(string channelSecretKey, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(channelSecretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);

        return Convert.ToBase64String(hash);
    }

    public static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public static LinePayResponseDto<LinePayPaymentResponseInfoDto>? GetPaymentResponse(this Order order)
    {
        _ = order.ExtraProperties.TryGetValue(PaymentGatewayConsts.PaymentResponse, out var response);

        var responseString = response?.ToString();

        return responseString != null ? Deserialize<LinePayPaymentResponseInfoDto>(responseString) : null;
    }

    public static LinePayResponseDto<LinePayConfirmResponseInfoDto>? GetConfirmPaymentResponse(this Order order)
    {
        _ = order.ExtraProperties.TryGetValue(PaymentGatewayConsts.ConfirmPaymentResponse, out var response);

        var responseString = response?.ToString();

        return responseString != null ? Deserialize<LinePayConfirmResponseInfoDto>(responseString) : null;
    }
}
