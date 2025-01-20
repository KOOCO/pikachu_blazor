using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayPaymentResponseInfoDto
{
    [JsonPropertyName("transactionId")]
    public JsonElement TransactionId { get; set; }

    [JsonIgnore]
    public BigInteger BigTransactionId
    {
        get
        {
            if (TransactionId.ValueKind != JsonValueKind.Undefined)
            {
                return TransactionId.ValueKind switch
                {
                    JsonValueKind.Number => new BigInteger(TransactionId.GetDecimal()),
                    JsonValueKind.String => BigInteger.Parse(TransactionId.GetString()),
                    _ => throw new JsonException("Invalid transactionId format.")
                };
            }

            return default;
        }
    }
    public string PaymentAccessToken { get; set; }
public LinePayPaymentResponseUrlDto PaymentUrl { get; set; }
}

public class LinePayPaymentResponseUrlDto
{
    public string? Web { get; set; }
    public string? App { get; set; }
}