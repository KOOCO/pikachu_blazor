namespace Kooco.Pikachu.Orders.Invoices;

/// <summary>
/// 稅務種類
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