namespace Kooco.Pikachu.Orders.Invoices;

/// <summary>
/// 發票創建方式
/// </summary>
public enum InvoiceCreation
{
    /// <summary>
    /// 手動建立
    /// </summary>
    Manual = 0,

    /// <summary>
    /// 依照排程自動建立
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// 重新開立
    /// </summary>
    Reopening = 2
}