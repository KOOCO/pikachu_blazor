using System;
using Volo.Abp.Application.Dtos;



namespace Kooco.Pikachu.Items.Dtos;

/// <summary>
/// 
/// </summary>
[Serializable]
public class ItemDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ItemId { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public long ItemNo { get; set; }

    /// <summary>
    /// 商品名稱        ItemName
    /// </summary>
    public string ItemName { get; set; }

    /// <summary>
    /// 商品描述        ItemDescription
    /// </summary>
    public string ItemDescription { get; set; }

    /// <summary>
    /// 商品售價        ItemSellingPrice
    /// </summary>
    public int SellingPrice { get; set; }

    /// <summary>
    /// 銷售帳戶        Sales Account
    /// </summary>
    public string SalesAccount { get; set; }

    /// <summary>
    /// 可否退貨        Returnable
    /// </summary>
    public Boolean Returnable { get; set; }

    /// <summary>
    /// 商品品牌名稱        Item Brand Name
    /// </summary>
    public string BrandName { get; set; }

    /// <summary>
    /// 商品製造商名稱        Item Manufactor Name
    /// </summary>
    public string ManufactorName { get; set; }

    /// <summary>
    /// 包裝寬度        Package Weight
    /// </summary>
    public short PackageWeight { get; set; }

    /// <summary>
    /// 包裝深度        Package Length
    /// </summary>
    public short PackageLength { get; set; }

    /// <summary>
    /// 包裝高度        Package Height
    /// </summary>
    public short PackageHeight { get; set; }

    /// <summary>
    /// 度量單位        Dimension Unit
    /// </summary>
    public Diemensions DiemensionsUnit { get; set; }

    /// <summary>
    /// 重量單位        Weight Unit
    /// </summary>
    public Weight WeightUnit { get; set; }

    /// <summary>
    /// 稅率名稱        Tax Name
    /// </summary>
    public string TaxName { get; set; }

    /// <summary>
    /// 稅率百分比        Tax Percentage
    /// </summary>
    public int TaxPercentage { get; set; }

    /// <summary>
    /// 稅率類型        Tax Type
    /// </summary>
    public string TaxType { get; set; }

    /// <summary>
    /// 採購稅率名稱        Purchase Tax Name
    /// </summary>
    public string PurchaseTaxName { get; set; }

    /// <summary>
    /// 採購稅率百分比        Purchase Tax Percentage
    /// </summary>
    public int PurchaseTaxPercentage { get; set; }

    /// <summary>
    /// 商品類型        Product Type
    /// </summary>
    public string ProductType { get; set; }

    /// <summary>
    /// 商品來源        Source
    /// </summary>
    public int Source { get; set; }

    /// <summary>
    /// 參考ID        Reference ID
    /// </summary>
    public int ReferenceID { get; set; }

    /// <summary>
    /// 最後同步時間        Last Sync Time
    /// </summary>
    public DateTime LastSyncTime { get; set; }

    /// <summary>
    /// 商品狀態        Status
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// 商品單位        Unit
    /// </summary>
    public Quantity Unit { get; set; }

    /// <summary>
    /// SKU
    /// </summary>
    public string SKU { get; set; }

    /// <summary>
    /// UPC
    /// </summary>
    public string UPC { get; set; }

    /// <summary>
    /// EAN
    /// </summary>
    public string EAN { get; set; }

    /// <summary>
    /// ISBN
    /// </summary>
    public string ISBN { get; set; }

    /// <summary>
    /// 部件編號        Part Number
    /// </summary>
    public string PartNumber { get; set; }

    /// <summary>
    /// 採購金額        Purchase Price
    /// </summary>
    public int PurchasePrice { get; set; }

    /// <summary>
    /// 採購帳戶        Purchase Account
    /// </summary>
    public string PurchaseAccount { get; set; }

    /// <summary>
    /// 採購描述        Purchase Description
    /// </summary>
    public string PurchaseDescription { get; set; }

    /// <summary>
    /// 庫存帳戶        Inventory Account
    /// </summary>
    public string InventoryAccount { get; set; }

    /// <summary>
    /// Reorder Level
    /// </summary>
    public int ReorderLevel { get; set; }

    /// <summary>
    /// 預設採購商        Preferred Vendor
    /// </summary>
    public string PreferredVendor { get; set; }

    /// <summary>
    /// 倉庫名稱        Warehouse Name
    /// </summary>
    public string WarehouseName { get; set; }

    /// <summary>
    /// 初始庫存        Opening Stock
    /// </summary>
    public int OpeningStock { get; set; }

    /// <summary>
    /// 初始庫存金額        Opening Stock Value
    /// </summary>
    public int OpeningStockValue { get; set; }

    /// <summary>
    /// Stock On Hand
    /// </summary>
    public int StockOnHand { get; set; }

    /// <summary>
    /// 是組合商品        Is Combo Product
    /// </summary>
    public bool IsComboProduct { get; set; }

    /// <summary>
    /// 商品歸戶        Item Type
    /// </summary>
    public string ItemType { get; set; }

    /// <summary>
    /// 商品類別        Item Category
    /// </summary>
    public string ItemCategory { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField1Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField1Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField2Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField2Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField3Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField3Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField4Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField4Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField5Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField5Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField6Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField6Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField7Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField7Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField8Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField8Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField9Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField9Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField10Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CustomeField10Value { get; set; }
}