using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayRefundResponseInfoDto
{
    public JsonElement RefundTransactionId { get; set; }

    [JsonIgnore]
    public BigInteger BigTransactionId
    {
        get
        {
            if (RefundTransactionId.ValueKind != JsonValueKind.Undefined)
            {
                return RefundTransactionId.ValueKind switch
                {
                    JsonValueKind.Number => new BigInteger(RefundTransactionId.GetDecimal()),
                    JsonValueKind.String => BigInteger.Parse(RefundTransactionId.GetString()),
                    _ => default
                };
            }

            return default;
        }
    }

    public DateTime RefundTransactionDate { get; set; }
}
