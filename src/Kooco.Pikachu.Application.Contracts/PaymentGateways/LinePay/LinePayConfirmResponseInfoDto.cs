using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayConfirmResponseInfoDto
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
                    _ => default
                };
            }

            return default;
        }
    }

    public string OrderId { get; set; }
    public List<LinePayConfirmResponsePayInfoDto> PayInfo { get; set; }
    public object Packages { get; set; }
}

public class LinePayConfirmResponsePayInfoDto
{
    public string Method { get; set; }
    public int Amount { get; set; }
    public string MaskedCreditCardNumber { get; set; }
}
