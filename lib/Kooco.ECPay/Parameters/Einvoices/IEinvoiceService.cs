using Microsoft.Extensions.DependencyInjection;
using Security.Cryptography;
using System.Text;
using System.Web;
using Text.Json;
using Volo.Abp.DependencyInjection;

namespace Kooco.Parameters.Einvoices;
internal interface IEinvoiceService
{
    Task<(int transCode, CreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV, CreateInvoiceInput input);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class EinvoiceService : IEinvoiceService
{
    public async Task<(int transCode, CreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV, CreateInvoiceInput input)
    {
        var json = input.ToJson(ECPayDefaults.JsonSerializerOptions);
        var encryptData = await Uri.EscapeDataString(json).AesEncryptAsync(hashKey, hashIV);
        var client = HttpClientFactory.CreateClient(nameof(ECPayConstants.Einvoice));
        StringContent request = new(new CreateInvoiceRequest
        {
            MerchantId = input.MerchantId,
            RequestHeader = new()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            },
            EncryptData = encryptData,
        }.ToJson(ECPayDefaults.JsonSerializerOptions), Encoding.UTF8, HttpContentType.ApplicationJson);
        var message = await client.PostAsync(ECPayConstants.Einvoice.B2CInvoicePath, request);
        var content = await message.Content.ReadAsStringAsync();
        var response = content.ToObject<CreateInvoiceResponse>();
        var result = await response.Data.AesDecryptAsync(hashKey, hashIV);

        var data = HttpUtility.UrlDecode(result);
        return (response.TransCode, data.ToObject<CreateInvoiceResult>());
    }

    public required IHttpClientFactory HttpClientFactory { get; init; }
}