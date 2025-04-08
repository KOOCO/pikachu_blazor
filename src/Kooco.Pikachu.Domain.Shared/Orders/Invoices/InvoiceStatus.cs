namespace Kooco.Pikachu.Orders.Invoices;

/// <summary>
/// 發票狀態
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    /// 開立中
    /// </summary>
    Issuing = 0,

    /// <summary>
    /// 已開立
    /// </summary>
    Issued = 1,

    /// <summary>
    /// 已作廢
    /// </summary>
    Voided = 2,
}