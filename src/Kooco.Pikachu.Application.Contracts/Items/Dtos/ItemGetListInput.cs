using System;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class ItemGetListInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    [DisplayName("ItemId")]
    public Guid? ItemId { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [DisplayName("ItemNo")]
    public long? ItemNo { get; set; }

    /// <summary>
    /// 商品名稱        ItemName
    /// </summary>
    [DisplayName("ItemName")]
    public string ItemName { get; set; }

    /// <summary>
    /// 商品敘述抬頭        ItemDescriptionTitle
    /// </summary>
    [DisplayName("ItemDescriptionTitle")]
    public string ItemDescriptionTitle { get; set; }

    /// <summary>
    /// 商品描述        ItemDescription
    /// </summary>
    [DisplayName("ItemDescription")]
    public string ItemDescription { get; set; }

    /// <summary>
    /// 商品分類1 ItemProperty1
    /// </summary>
    [DisplayName("Property")]
    public string? Property1 { get; set; }

     /// <summary>
     /// 商品分類2 ItemProperty2
     /// </summary>
     [DisplayName("Property")]
    public string? Property2 { get; set; }

    /// <summary>
    /// 商品售價        ItemSellingPrice
    /// </summary>
    [DisplayName("ItemSellingPrice")]
    public int? SellingPrice { get; set; }

    /// <summary>
    /// 銷售帳戶        Sales Account
    /// </summary>
    [DisplayName("ItemSalesAccount")]
    public string SalesAccount { get; set; }

    /// <summary>
    /// 可否退貨        Returnable
    /// </summary>
    [DisplayName("ItemReturnable")]
    public Boolean? Returnable { get; set; }

              /// <summary>
        /// 限時販售開始時間 Ｌimit Avaliable Time Start
        /// </summary>
        [DisplayName("LimitAvaliableTimeStart")]
        public DateTime LimitAvaliableTimeStart { get; set; }

        /// <summary>
        /// 限時販售結束時間 Ｌimit Avaliable Time End
        /// </summary>
        [DisplayName("LimitAvaliableTimeEnd")]
        public DateTime LimitAvaliableTimeEnd { get; set; }

        /// <summary>
        /// 販售數量限制
        /// </summary>
        [DisplayName("QuantityLimit")]
        public int QuantityLimit { get; set; }

        /// <summary>
        /// 分潤 Share Profit
        /// </summary>
        [DisplayName("ShareProfit")]
        public int ShareProfit { get; set; }

        /// <summary>
        /// 是否免運 Is Free Shipping
        /// </summary>
        [DisplayName("isFreeShipping")]
        public bool isFreeShipping { get; set; } = false;

        /// <summary>
        /// 排除運送方式 Exclude Shipping Method
        /// </summary>
        [DisplayName("ExclueShippingMethod")]
        public Array ExclueShippingMethod { get; set; }


    /// <summary>
    /// 商品品牌名稱        Item Brand Name
    /// </summary>
    [DisplayName("ItemBrandName")]
    public string BrandName { get; set; }

    /// <summary>
    /// 商品製造商名稱        Item Manufactor Name
    /// </summary>
    [DisplayName("ItemManufactorName")]
    public string ManufactorName { get; set; }

    /// <summary>
    /// 包裝寬度        Package Weight
    /// </summary>
    [DisplayName("ItemPackageWeight")]
    public short? PackageWeight { get; set; }

    /// <summary>
    /// 包裝深度        Package Length
    /// </summary>
    [DisplayName("ItemPackageLength")]
    public short? PackageLength { get; set; }

    /// <summary>
    /// 包裝高度        Package Height
    /// </summary>
    [DisplayName("ItemPackageHeight")]
    public short? PackageHeight { get; set; }

    /// <summary>
    /// 度量單位        Dimension Unit
    /// </summary>
    [DisplayName("ItemDiemensionsUnit")]
    public Diemensions? DiemensionsUnit { get; set; }

    /// <summary>
    /// 重量單位        Weight Unit
    /// </summary>
    [DisplayName("ItemWeightUnit")]
    public Weight? WeightUnit { get; set; }

    /// <summary>
    /// 稅率名稱        Tax Name
    /// </summary>
    [DisplayName("ItemTaxName")]
    public string TaxName { get; set; }

    /// <summary>
    /// 稅率百分比        Tax Percentage
    /// </summary>
    [DisplayName("ItemTaxPercentage")]
    public int? TaxPercentage { get; set; }

    /// <summary>
    /// 稅率類型        Tax Type
    /// </summary>
    [DisplayName("ItemTaxType")]
    public string TaxType { get; set; }

    /// <summary>
    /// 採購稅率名稱        Purchase Tax Name
    /// </summary>
    [DisplayName("ItemPurchaseTaxName")]
    public string PurchaseTaxName { get; set; }

    /// <summary>
    /// 採購稅率百分比        Purchase Tax Percentage
    /// </summary>
    [DisplayName("ItemPurchaseTaxPercentage")]
    public int? PurchaseTaxPercentage { get; set; }

    /// <summary>
    /// 商品類型        Product Type
    /// </summary>
    [DisplayName("ItemProductType")]
    public string ProductType { get; set; }

    /// <summary>
    /// 商品來源        Source
    /// </summary>
    [DisplayName("ItemSource")]
    public int? Source { get; set; }

    /// <summary>
    /// 參考ID        Reference ID
    /// </summary>
    [DisplayName("ItemReferenceID")]
    public int? ReferenceID { get; set; }

    /// <summary>
    /// 最後同步時間        Last Sync Time
    /// </summary>
    [DisplayName("ItemLastSyncTime")]
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// 商品狀態        Status
    /// </summary>
    [DisplayName("ItemStatus")]
    public string Status { get; set; }

    /// <summary>
    /// 商品單位        Unit
    /// </summary>
    [DisplayName("ItemUnit")]
    public Quantity? Unit { get; set; }

    /// <summary>
    /// SKU
    /// </summary>
    [DisplayName("ItemSKU")]
    public string SKU { get; set; }

    /// <summary>
    /// UPC
    /// </summary>
    [DisplayName("ItemUPC")]
    public string UPC { get; set; }

    /// <summary>
    /// EAN
    /// </summary>
    [DisplayName("ItemEAN")]
    public string EAN { get; set; }

    /// <summary>
    /// ISBN
    /// </summary>
    [DisplayName("ItemISBN")]
    public string ISBN { get; set; }

    /// <summary>
    /// 部件編號        Part Number
    /// </summary>
    [DisplayName("ItemPartNumber")]
    public string PartNumber { get; set; }

    /// <summary>
    /// 採購金額        Purchase Price
    /// </summary>
    [DisplayName("ItemPurchasePrice")]
    public int? PurchasePrice { get; set; }

    /// <summary>
    /// 採購帳戶        Purchase Account
    /// </summary>
    [DisplayName("ItemPurchaseAccount")]
    public string PurchaseAccount { get; set; }

    /// <summary>
    /// 採購描述        Purchase Description
    /// </summary>
    [DisplayName("ItemPurchaseDescription")]
    public string PurchaseDescription { get; set; }

    /// <summary>
    /// 庫存帳戶        Inventory Account
    /// </summary>
    [DisplayName("ItemInventoryAccount")]
    public string InventoryAccount { get; set; }

    /// <summary>
    /// Reorder Level
    /// </summary>
    [DisplayName("ItemReorderLevel")]
    public int? ReorderLevel { get; set; }

    /// <summary>
    /// 預設採購商        Preferred Vendor
    /// </summary>
    [DisplayName("ItemPreferredVendor")]
    public string PreferredVendor { get; set; }

    /// <summary>
    /// 倉庫名稱        Warehouse Name
    /// </summary>
    [DisplayName("ItemWarehouseName")]
    public string WarehouseName { get; set; }

    /// <summary>
    /// 初始庫存        Opening Stock
    /// </summary>
    [DisplayName("ItemOpeningStock")]
    public int? OpeningStock { get; set; }

    /// <summary>
    /// 初始庫存金額        Opening Stock Value
    /// </summary>
    [DisplayName("ItemOpeningStockValue")]
    public int? OpeningStockValue { get; set; }

    /// <summary>
    /// Stock On Hand
    /// </summary>
    [DisplayName("ItemStockOnHand")]
    public int? StockOnHand { get; set; }

    /// <summary>
    /// 是組合商品        Is Combo Product
    /// </summary>
    [DisplayName("ItemIsComboProduct")]
    public bool? IsComboProduct { get; set; }

    /// <summary>
    /// 商品歸戶        Item Type
    /// </summary>
    [DisplayName("ItemType")]
    public string ItemType { get; set; }

    /// <summary>
    /// 商品類別        Item Category
    /// </summary>
    [DisplayName("ItemCategory")]
    public string ItemCategory { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField1Name")]
    public string CustomeField1Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField1Value")]
    public string CustomeField1Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField2Name")]
    public string CustomeField2Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField2Value")]
    public string CustomeField2Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField3Name")]
    public string CustomeField3Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField3Value")]
    public string CustomeField3Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField4Name")]
    public string CustomeField4Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField4Value")]
    public string CustomeField4Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField5Name")]
    public string CustomeField5Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField5Value")]
    public string CustomeField5Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField6Name")]
    public string CustomeField6Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField6Value")]
    public string CustomeField6Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField7Name")]
    public string CustomeField7Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField7Value")]
    public string CustomeField7Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField8Name")]
    public string CustomeField8Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField8Value")]
    public string CustomeField8Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField9Name")]
    public string CustomeField9Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField9Value")]
    public string CustomeField9Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField10Name")]
    public string CustomeField10Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemCustomeField10Value")]
    public string CustomeField10Value { get; set; }
}