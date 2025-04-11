using System.Text.Json.Serialization;

namespace Kooco.Parameters.Einvoices;
public sealed class CreateInvoiceRequest
{
    [JsonPropertyName("PlatformID")]
    public string? PlatformId { get; set; }

    [JsonPropertyName("MerchantID")]
    public required string MerchantId { get; set; }

    [JsonPropertyName("RqHeader")]
    public required HeaderInfo RequestHeader { get; set; }

    [JsonPropertyName("Data")]
    public required string EncryptData { get; set; }

    public sealed class HeaderInfo
    {
        public long Timestamp { get; set; }
    }
}