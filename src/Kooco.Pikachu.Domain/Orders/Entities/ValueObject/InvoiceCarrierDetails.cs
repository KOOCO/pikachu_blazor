namespace Kooco.Pikachu.Orders.Entities.ValueObject;
public class InvoiceCarrierDetails
{
    public CarrierType Type { get; set; }
    public string CarrierNo { get; set; }

    public enum CarrierType
    {
        /// <summary>
        /// 綠界電子發票載具
        /// </summary>
        ECPay = 1,

        /// <summary>
        /// 自然人憑證號碼
        /// </summary>
        Personal = 2,

        /// <summary>
        /// 手機條碼載具
        /// </summary>
        MobileBarcode = 3,
    }
}