namespace Kooco.Pikachu.Orders.Invoices;

/// <summary>
/// 課稅類型
/// </summary>
public enum InvoiceTaxType
{
    /// <summary>
    /// 應稅
    /// </summary>
    Taxable = 1,

    /// <summary>
    /// 零稅率
    /// </summary>
    ZeroTax = 2,

    /// <summary>
    /// 免稅
    /// </summary>
    DutyFree = 3
}