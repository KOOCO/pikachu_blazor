namespace Kooco.Pikachu.Orders.Invoices;

/// <summary>
/// 發票開立方式
/// </summary>
public enum InvoiceIssuanceMethod
{
    /// <summary>
    /// 紙本
    /// </summary>
    Paper,

    /// <summary>
    /// 個人
    /// </summary>
    Personal,

    /// <summary>
    /// 載具
    /// </summary>
    Carrier,

    /// <summary>
    /// 捐贈
    /// </summary>
    Donation,
}