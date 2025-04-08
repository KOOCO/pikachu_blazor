namespace Kooco.Pikachu.Orders.Invoices;

/// <summary>
/// 發票創建方式
/// </summary>
public enum InvoiceCreation
{
    /// <summary>
    /// 手動建立
    /// </summary>
    Manual,

    /// <summary>
    /// 依照排程自動建立
    /// </summary>
    Scheduled,

    /// <summary>
    /// 重新開立
    /// </summary>
    Reopening
}