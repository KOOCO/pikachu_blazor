using System;

namespace Kooco.Pikachu.Orders.ValueObject;
public sealed class InvoiceIssuedDetails
{
    /// <summary>
    /// 開立時間
    /// </summary>
    public required DateTime IssueTime { get; set; }
}