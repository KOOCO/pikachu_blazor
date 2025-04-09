using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using System.Web;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Einvoices;
public interface IECPayInvoiceService
{
    Task<(int transCode, ECPayCreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV, ECPayCreateInvoiceInput input);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ECPayInvoiceService : IECPayInvoiceService
{
    public async Task<(int transCode, ECPayCreateInvoiceResult result)> CreateInvoiceAsync(string hashKey, string hashIV, ECPayCreateInvoiceInput input)
    {
        var json = JsonSerializer.Serialize(input, ECPayDefaults.JsonSerializerOptions);
        var encryptData = await Uri.EscapeDataString(json).AesEncryptAsync(hashKey, hashIV);
        var client = HttpClientFactory.CreateClient(nameof(ECPayConstants.Einvoice));
        var payload = JsonSerializer.Serialize(new ECPayCreateInvoiceRequest()
        {
            MerchantId = input.MerchantId,
            RequestHeader = new()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            },
            EncryptData = encryptData,
        }, ECPayDefaults.JsonSerializerOptions);
        StringContent request = new(payload, Encoding.UTF8, ECPayConstants.MediaType);
        var message = await client.PostAsync(ECPayConstants.Einvoice.B2CInvoicePath, request);
        var content = await message.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<ECPayCreateInvoiceResponse>(content);
        var result = await response.Data.AesDecryptAsync(hashKey, hashIV);

        var data = HttpUtility.UrlDecode(result);
        return (response.TransCode, JsonSerializer.Deserialize<ECPayCreateInvoiceResult>(data));
    }

    public required IHttpClientFactory HttpClientFactory { get; init; }
}