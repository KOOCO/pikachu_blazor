using System.Text.Json.Serialization;

namespace Kooco.Pikachu.Einvoices;
public sealed class ECPayCreateInvoiceInput
{
    /// <summary>
    /// 特店編號 String(10)
    /// </summary>
    [JsonPropertyName("MerchantID")]
    public string MerchantId { get; set; }

    /// <summary>
    /// 關聯編號
    /// </summary>
    [JsonPropertyName("RelateNumber")]
    public string RelateNo { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// 客戶地址
    /// </summary>
    public string CustomerAddr { get; set; }

    /// <summary>
    /// 客戶電話
    /// </summary>
    public string CustomerPhone { get; set; }

    /// <summary>
    /// 客戶電子郵件
    /// </summary>
    public string CustomerEmail { get; set; }

    /// <summary>
    /// 清關標記
    /// </summary>
    public string ClearanceMark { get; set; }

    /// <summary>
    /// 發票備註
    /// </summary>
    public string InvoiceRemark { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? CustomerIdentifier { get; set; }

    /// <summary>
    /// 列印標記
    /// </summary>
    public string Print { get; set; }

    /// <summary>
    /// 承運商編號（可為空）
    /// </summary>
    public string? CarrierNum { get; set; }

    /// <summary>
    /// 承運商類型（可為空）
    /// </summary>
    public string? CarrierType { get; set; }

    /// <summary>
    /// 捐贈標記
    /// </summary>
    public string Donation { get; set; }

    /// <summary>
    /// 稅務類型
    /// </summary>
    public string TaxType { get; set; }

    // 銷售金額
    public decimal SalesAmount { get; set; }

    // 發票類型
    public string InvType { get; set; }

    [JsonPropertyName("vat")]
    public string Vat { get; set; }
    public List<Item> Items { get; set; }

    public class Item
    {
        public int ItemSeq { get; set; }
        public string ItemName { get; set; }
        public int ItemCount { get; set; }
        public string ItemWord { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemTaxType { get; set; }
        public decimal ItemAmount { get; set; }
        public string ItemRemark { get; set; }
    }
}