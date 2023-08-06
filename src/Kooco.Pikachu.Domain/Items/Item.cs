using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class Item : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 商品編號
        /// </summary>
        public long ItemNo { get; set; }

        /// <summary>
        /// 商品名稱
        /// ItemName
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 商品敘述抬頭 ItemDescriptionTitle
        /// </summary>
        public string? ItemDescriptionTitle { get; set; }
        /// <summary>
        /// 商品描述
        /// ItemDescription
        /// </summary>
        public string? ItemDescription { get; set; }

        /// <summary>
        /// 商品分類1 ItemProperty1
        /// </summary>
        public string? Property1 { get; set; }

        /// <summary>
        /// 商品分類2 ItemProperty2
        /// </summary>
        public string? Property2 { get; set; }
        /// <summary>
        /// 商品售價
        /// ItemSellingPrice
        /// </summary>
        public int SellingPrice { get; set; }

        /// <summary>
        /// 商品團購價 Group Buy Price
        /// </summary>
        public int? GroupBuyPrice { get; set; }

        /// <summary>
        /// 銷售帳戶
        /// Sales Account
        /// </summary>
        public string? SalesAccount { get; set; }
        /// <summary>
        /// 可否退貨
        /// Returnable
        /// </summary>
        public Boolean Returnable { get; set; } = false;

        /// <summary>
        /// 限時販售開始時間 Ｌimit Avaliable Time Start
        /// </summary>
        public DateTime LimitAvaliableTimeStart { get; set; }

        /// <summary>
        /// 限時販售結束時間 Ｌimit Avaliable Time End
        /// </summary>
        public DateTime LimitAvaliableTimeEnd { get; set; }

        /// <summary>
        /// 販售數量限制
        /// </summary>
        public int QuantityLimit { get; set; }

        /// <summary>
        /// 分潤 Share Profit
        /// </summary>
        public int ShareProfit { get; set; }

        /// <summary>
        /// 是否免運 Is Free Shipping
        /// </summary>
        public bool isFreeShipping { get; set; } = false;

        /// <summary>
        /// 排除運送方式 Exclude Shipping Method
        /// </summary>
        public Array ExclueShippingMethod { get; set; }

        /// <summary>
        /// 商品品牌名稱
        /// Item Brand Name
        /// </summary>
        public string? BrandName { get; set; }
        /// <summary>
        /// 商品製造商名稱
        /// Item Manufactor Name
        /// </summary>
        public string? ManufactorName { get; set; }
        /// <summary>
        /// 包裝寬度
        /// Package Weight
        /// </summary>
        public short? PackageWeight { get; set; }
        /// <summary>
        /// 包裝深度
        /// Package Length
        /// </summary>
        public short? PackageLength { get; set; }
        /// <summary>
        /// 包裝高度
        /// Package Height
        /// </summary>
        public short? PackageHeight { get; set; }
        /// <summary>
        /// 度量單位
        /// Dimension Unit
        /// </summary>
        public Diemensions? DiemensionsUnit { get; set; }
        /// <summary>
        /// 重量單位
        /// Weight Unit
        /// </summary>
        public Weight? WeightUnit { get; set; }
        /// <summary>
        /// 稅率名稱
        /// Tax Name
        /// </summary>
        public string? TaxName { get; set; }
        /// <summary>
        /// 稅率百分比
        /// Tax Percentage
        /// </summary>
        public int? TaxPercentage { get; set; }
        /// <summary>
        /// 商品稅別
        /// Tax Type
        /// </summary>
        public string? TaxType { get; set; }
        /// <summary>
        /// 採購稅率名稱
        /// Purchase Tax Name
        /// </summary>
        public string? PurchaseTaxName { get; set; }
        /// <summary>
        /// 採購稅率百分比
        /// Purchase Tax Percentage
        /// </summary>
        public int? PurchaseTaxPercentage { get; set; }
        /// <summary>
        /// 商品類型
        /// Product Type
        /// </summary>
        public string? ProductType { get; set; }
        /// <summary>
        /// 商品來源
        /// Source
        /// </summary>
        public int? Source { get; set; }
        /// <summary>
        /// 參考ID
        /// Reference ID
        /// </summary>
        public int? ReferenceID { get; set; }
        /// <summary>
        /// 最後同步時間
        /// Last Sync Time
        /// </summary>
        public DateTime? LastSyncTime { get; set; }
        /// <summary>
        /// 商品狀態
        /// Status
        /// </summary>
        public string? Status { get; set; }
        /// <summary>
        /// 商品單位
        /// Unit
        /// </summary>
        public Quantity? Unit { get; set; }
        /// <summary>
        /// SKU
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// UPC
        /// </summary>
        public string? UPC { get; set; }
        /// <summary>
        /// EAN
        /// </summary>
        public string? EAN { get; set; }
        /// <summary>
        /// ISBN
        /// </summary>
        public string? ISBN { get; set; }
        /// <summary>
        /// 部件編號
        /// Part Number
        /// </summary>
        public string? PartNumber { get; set; }
        /// <summary>
        /// 採購金額
        /// Purchase Price
        /// </summary>
        public int? PurchasePrice { get; set; }
        /// <summary>
        /// 採購帳戶
        /// Purchase Account
        /// </summary>
        public string? PurchaseAccount { get; set; }
        /// <summary>
        /// 採購描述
        /// Purchase Description
        /// </summary>
        public string? PurchaseDescription { get; set; }
        /// <summary>
        /// 庫存帳戶
        /// Inventory Account
        /// </summary>
        public string? InventoryAccount { get; set; }
        /// <summary>
        /// Reorder Level
        /// </summary>
        public int? ReorderLevel { get; set; }
        /// <summary>
        /// 預設採購商
        /// Preferred Vendor
        /// </summary>
        public string? PreferredVendor { get; set; }
        /// <summary>
        /// 倉庫名稱
        /// Warehouse Name
        /// </summary>
        public string? WarehouseName { get; set; }
        /// <summary>
        /// 初始庫存
        /// Opening Stock
        /// </summary>
        public int? OpeningStock { get; set; }
        /// <summary>
        /// 初始庫存金額
        /// Opening Stock Value
        /// </summary>
        public int OpeningStockValue { get; set; }
        /// <summary>
        /// 現有庫存 - 這是統計全部倉庫的庫存
        /// Stock On Hand - This is the total stock of all warehouses
        /// </summary>
        public int? StockOnHand { get; set; } = 0;
        /// <summary>
        /// 是組合商品
        /// Is Group Product
        /// </summary>
        public bool? IsGroupProduct { get; set; }
        /// <summary>
        /// 商品歸戶
        /// Item Type
        /// </summary>
        public string? ItemType { get; set; }
        /// <summary>
        /// 商品類別
        /// Item Category
        /// </summary>
        public string? ItemCategory { get; set; }
        public string? CustomeField1Name { get; set; }
        public string? CustomeField1Value { get; set; }
        public string? CustomeField2Name { get; set; }
        public string? CustomeField2Value { get; set; }
        public string? CustomeField3Name { get; set; }
        public string? CustomeField3Value { get; set; }
        public string? CustomeField4Name { get; set; }
        public string? CustomeField4Value { get; set; }
        public string? CustomeField5Name { get; set; }
        public string? CustomeField5Value { get; set; }
        public string? CustomeField6Name { get; set; }
        public string? CustomeField6Value { get; set; }
        public string? CustomeField7Name { get; set; }
        public string? CustomeField7Value { get; set; }
        public string? CustomeField8Name { get; set; }
        public string? CustomeField8Value { get; set; }
        public string? CustomeField9Name { get; set; }
        public string? CustomeField9Value { get; set; }
        public string? CustomeField10Name { get; set; }
        public string? CustomeField10Value { get; set; }

    }
}
