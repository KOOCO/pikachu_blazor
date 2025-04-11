using Microsoft.Extensions.DependencyInjection;
using Security.Cryptography;
using System.Text;
using System.Web;
using Text.Json;
using Volo.Abp.DependencyInjection;

namespace Kooco.Invoices;
public interface IECPayInvoiceService
{
    /// <summary>
    /// 發票開立
    /// </summary>
    Task<(int transCode, ECPayCreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV, ECPayCreateInvoiceInput input);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ECPayInvoiceService : IECPayInvoiceService
{
    public async Task<(int transCode, ECPayCreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV, ECPayCreateInvoiceInput input)
    {
        var json = input.ToJson(ECPayDefaults.JsonSerializerOptions);
        StringContent request = new(new ECPayCreateInvoiceRequest
        {
            MerchantId = input.MerchantId,
            RequestHeader = new()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            },
            EncryptData = await Uri.EscapeDataString(json).AesEncryptAsync(hashKey, hashIV),
        }.ToJson(ECPayDefaults.JsonSerializerOptions), Encoding.UTF8, HttpContentType.ApplicationJson);

        var client = HttpClientFactory.CreateClient(nameof(ECPayConstants.Einvoice));
        var message = await client.PostAsync(ECPayConstants.Einvoice.B2CInvoicePath, request);
        var content = await message.Content.ReadAsStringAsync();
        var response = content.ToObject<ECPayCreateInvoiceResponse>();
        var result = await response.Data.AesDecryptAsync(hashKey, hashIV);
        var data = HttpUtility.UrlDecode(result);
        return (response.TransCode, data.ToObject<ECPayCreateInvoiceResult>());
    }

    public required IHttpClientFactory HttpClientFactory { get; init; }
}