using System;

namespace Kooco.Pikachu.Orders.ValueObject;
public sealed class InvoiceVoidedDetails
{
    /// <summary>
    /// 作廢原因
    /// </summary>
    public required string VoidReason { get; set; }

    /// <summary>
    /// 作廢時間
    /// </summary>
    public required DateTime VoidedTime { get; set; }
}