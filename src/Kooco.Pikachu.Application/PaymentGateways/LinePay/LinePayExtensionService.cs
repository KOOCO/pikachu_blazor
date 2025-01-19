using Kooco.Pikachu.Orders;
using Microsoft.Extensions.Localization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public static class LinePayExtensionService
{
    public static string CreatePaymentRequest(this Order order, LinePayConfiguration apiOptions, IStringLocalizer l)
    {
        var packages = new List<LinePayPaymentRequestPackageDto>
        {
            new() {
                Id = order.OrderNo,
                Amount = Convert.ToInt32(order.OrderItems.Sum(oi => oi.TotalAmount)),
                Name = order.GroupBuy?.GroupBuyName,
                Products = order.OrderItems.Select(oi => new LinePayPaymentRequestProductDto
                {
                    Id = oi.Id.ToString(),
                    Name = oi.ItemId.HasValue ? oi.Item?.ItemName : oi.SetItem?.SetItemName,
                    ImageUrl = oi.ItemId.HasValue ? oi.Item?.ItemMainImageURL : oi.SetItem?.SetItemMainImageURL,
                    Quantity = oi.Quantity,
                    Price = Convert.ToInt32(oi.ItemPrice)
                }).ToList()
            }
        };

        var deliveryCosts = new[]
        {
            new { Label = l["DeliveryCost"], Cost = order.DeliveryCost },
            new { Label = l["DeliveryCostForNormal"], Cost = order.DeliveryCostForNormal },
            new { Label = l["DeliveryCostForFreeze"], Cost = order.DeliveryCostForFreeze },
            new { Label = l["DeliveryCostForFrozen"], Cost = order.DeliveryCostForFrozen }
        };

        var totalCost = deliveryCosts.Sum(d => d.Cost);

        if (totalCost > 0)
        {
            var deliveryPackages = new[]
            {
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
            };

            packages = [.. packages, .. deliveryPackages];
        }

        var body = new LinePayPaymentRequestDto
        {
            Amount = (int)order.TotalAmount,
            Currency = "TWD",
            OrderId = order.OrderNo,
            Packages = packages,
            RedirectUrls = new LinePayPaymentRequestRedirectUrlDto
            {
                ConfirmUrl = $"{apiOptions.SelfUrl}/api/app/line-pay/confirm",
                CancelUrl = $"{apiOptions.SelfUrl}/api/app/line-pay/cancel"
            }
        };

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

    public static LinePayResponseDto<LinePayPaymentResponseInfoDto> DeserializePaymentRequest(RestResponse restResponse)
    {
        var responseContent = restResponse.Content;
        var responseDto = (responseContent == null ? new()
            : JsonSerializer.Deserialize<LinePayResponseDto<LinePayPaymentResponseInfoDto>>(responseContent, GetJsonOptions()))
            ?? new();

        responseDto.IsSuccessful = restResponse.IsSuccessful;
        return responseDto;
    }

    public static string GeneratePostSignature(LinePayConfiguration apiOptions, string channelSecretKey, string serializedBody, string nonce)
    {
        var signatureBase = channelSecretKey + apiOptions.PaymentApiPath + serializedBody + nonce;
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

    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
