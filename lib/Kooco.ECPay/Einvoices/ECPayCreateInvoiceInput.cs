using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kooco.Einvoices;
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
        /// <summary>
        /// 商品序號 (非必要)
        /// 用於區分商品項目，但 ECPay API 文件通常不強制要求此欄位。
        /// 如果您在自己的系統中需要追蹤項目順序，可以使用此欄位。
        /// </summary>
        public int? ItemSeq { get; set; } // 改為 nullable int，因為通常非必要

        /// <summary>
        /// 商品名稱 (必要)
        /// 例：氣泡水、網站設計服務
        /// </summary>
        [Required(ErrorMessage = "商品名稱 (ItemName) 為必填")] // 範例：加上資料驗證
        public string ItemName { get; set; }

        /// <summary>
        /// 商品數量 (必要)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "商品數量 (ItemCount) 必須大於 0")] // 範例：加上資料驗證
        public int ItemCount { get; set; }

        /// <summary>
        /// 商品單位 (必要)
        /// 例：瓶、個、式、組
        /// </summary>
        [Required(ErrorMessage = "商品單位 (ItemWord) 為必填")]
        public string ItemWord { get; set; }

        /// <summary>
        /// 商品單價 (必要)
        /// 注意：必須是未稅金額 (如果 TaxType 為 1 應稅) 或含稅金額 (如果 TaxType 為 3 免稅)。
        /// ECPay 文件會詳細說明，請務必參考。通常建議傳送 *未稅* 單價，讓 ECPay 計算稅額。
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "商品單價 (ItemPrice) 不可為負數")]
        public decimal ItemPrice { get; set; }

        /// <summary>
        /// 商品課稅別 (必要)
        /// 1: 應稅 (需計算營業稅)
        /// 2: 零稅率 (營業稅率為 0%)
        /// 3: 免稅 (依法免徵營業稅)
        /// 9: 混合應稅與免稅或零稅率 (僅在發票類型為 07 特種稅額計算時使用)
        /// 請務必參考 ECPay 最新的官方文件確認數值意義。
        /// </summary>
        [Required(ErrorMessage = "商品課稅別 (ItemTaxType) 為必填")]
        public int ItemTaxType { get; set; } // 保持 int，因為 ECPay API 定義是 int

        /// <summary>
        /// 商品合計金額 (必要)
        /// 此項目的總金額，通常等於 ItemCount * ItemPrice。
        /// 注意：此金額應為 *未稅* 總額 (如果 TaxType 為 1 應稅) 或 *含稅* 總額 (如果 TaxType 為 3 免稅)。
        /// 與 ItemPrice 一樣，請務必參考 ECPay 文件，通常建議傳送 *未稅* 合計。
        /// ECPay 系統會根據所有商品的 ItemAmount 和 ItemTaxType 來計算最終的稅額與總計金額。
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "商品合計金額 (ItemAmount) 不可為負數")]
        public decimal ItemAmount { get; set; }

        /// <summary>
        /// 商品備註 (非必要)
        /// 可在此欄位填寫此商品的特殊說明，例如：商品序號、優惠說明等。
        /// 會顯示在電子發票證明聯的「備註」欄位。
        /// </summary>
        public string ItemRemark { get; set; }
    }
}