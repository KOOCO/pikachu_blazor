using Refit;
using System.Text.Json.Serialization;

namespace Kooco.Pikachu.Parameters;
public class EinvoiceRequest
{
    [AliasAs("PlatformID")]
    public string? PlatformId { get; set; }

    [AliasAs("MerchantID")]
    public required string MerchantId { get; set; }

    [AliasAs("RqHeader")]
    public required HeaderInfo RequestHeader { get; set; }

    [AliasAs("Data")]
    public required string EncryptData { get; set; }

    public class HeaderInfo
    {
        public long Timestamp { get; set; }
    }

    public class DataInfo
    {
        /// <summary>
        /// 特店編號 String(10)
        /// </summary>
        public required string MerchantID { get; set; }

        /// <summary>
        /// 特店自訂編號 String(30)
        /// </summary>
        public required string RelateNumber { get; set; }

        /// <summary>
        /// 通路商編號 String(1)
        /// </summary>
        public string? ChannelPartner { get; set; }

        /// <summary>
        /// 客戶編號 String(20)
        /// </summary>
        public string? CustomerID { get; set; }

        /// <summary>
        /// 產品服務別代號 String(10)
        /// </summary>
        public string? ProductServiceID { get; set; }

        /// <summary>
        /// 統一編號 String(8)
        /// </summary>
        public string? CustomerIdentifier { get; set; }

        /// <summary>
        /// 客戶名稱 String(60)
        /// </summary>
        public required string CustomerName { get; set; }

        /// <summary>
        /// 客戶地址 String(100)
        /// </summary>
        public required string CustomerAddr { get; set; }

        /// <summary>
        /// 客戶電話 String(20)
        /// </summary>
        public required string CustomerPhone { get; set; }

        /// <summary>
        /// 客戶電子郵件 String(100)
        /// </summary>
        public required string CustomerEmail { get; set; }

        /// <summary>
        /// 通關方式 String(1)
        /// </summary>
        public required string ClearanceMark { get; set; }

        /// <summary>
        /// 列印註記 String(1)
        /// </summary>
        public string Print { get; set; }

        /// <summary>
        /// 捐贈註記 String(1)
        /// </summary>
        public required string Donation { get; set; }

        /// <summary>
        /// 捐贈碼 String(7)
        /// </summary>
        public string LoveCode { get; set; }

        /// <summary>
        /// 載具類別 String(1)
        /// </summary>
        public string? CarrierType { get; set; }

        /// <summary>
        /// 載具編號 String(64)
        /// </summary>
        public string? CarrierNum { get; set; }

        /// <summary>
        /// 課稅類別 String(1)
        /// </summary>
        public string TaxType { get; set; }

        /// <summary>
        /// 零稅率原因 String(2)
        /// </summary>
        public string ZeroTaxRateReason { get; set; }

        /// <summary>
        /// 特種稅額類別
        /// </summary>
        public int SpecialTaxType { get; set; }

        /// <summary>
        /// 發票總金額(含稅)
        /// </summary>
        public decimal SalesAmount { get; set; }

        /// <summary>
        /// 發票備註 String(200)
        /// </summary>
        public string InvoiceRemark { get; set; }

        /// <summary>
        /// 字軌類別 String(2)
        /// </summary>
        public required string InvType { get; set; }

        /// <summary>
        /// 商品單價是否含稅 String(1)
        /// </summary>
        [JsonPropertyName("vat")]
        public string Vat { get; set; }

        public List<Item> Items { get; set; }
        public class Item
        {
            /// <summary>
            /// 商品序號
            /// </summary>
            public int ItemSeq { get; set; }

            /// <summary>
            /// 商品名稱 String(100)
            /// </summary>
            public required string ItemName { get; set; }

            /// <summary>
            /// 商品數量
            /// </summary>
            public required int ItemCount { get; set; }

            /// <summary>
            /// 商品單位 String(6)
            /// </summary>
            public required string ItemWord { get; set; }

            /// <summary>
            ///  商品單價
            /// </summary>
            public required decimal ItemPrice { get; set; }

            /// <summary>
            /// 商品課稅別
            /// </summary>
            public int ItemTaxType { get; set; }

            /// <summary>
            /// 商品合計
            /// </summary>
            public required decimal ItemAmount { get; set; }

            /// <summary>
            /// 商品備註 String(40)
            /// </summary>
            public string ItemRemark { get; set; }
        }
    }
}