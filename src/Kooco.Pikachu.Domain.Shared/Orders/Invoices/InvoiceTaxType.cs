namespace Kooco.Pikachu.Orders.Invoices;

/// <summary>
/// 課稅類型
/// </summary>
public enum InvoiceTaxType
{
    /// <summary>
    /// 應稅
    /// </summary>
    Taxable,

    /// <summary>
    /// 零稅率
    /// </summary>
    ZeroTax,

    /// <summary>
    /// 免稅
    /// </summary>
    DutyFree,
}