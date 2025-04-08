namespace Kooco.Pikachu.Orders.ValueObject;
public sealed class InvoiceCarrierDetails
{
    /// <summary>
    /// 載具類型
    /// </summary>
    public required CarrierType Type { get; set; }

    /// <summary>
    /// 載具號碼
    /// </summary>
    public required string CarrierNo { get; set; }

    public enum CarrierType
    {
        /// <summary>
        /// 綠界電子發票載具
        /// </summary>
        ECPay,

        /// <summary>
        /// 自然人憑證號碼
        /// </summary>
        Personal,

        /// <summary>
        /// 手機條碼載具
        /// </summary>
        MobileBarcode,
    }
}