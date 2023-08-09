using System;
using System.ComponentModel;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateUpdateItemDetailsDto
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemDetailsItem")]
    public ItemDto Item { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemDetailsItemDetailTitle")]
    public string? ItemDetailTitle { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemDetailsItemDetailStatus")]
    public string? ItemDetailStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemDetailsItemDetailDescription")]
    public string? ItemDetailDescription { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemDetailsProperty1")]
    public string? Property1 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemDetailsProperty2")]
    public string? Property2 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("ItemDetailsProperty3")]
    public string? Property3 { get; set; }

    /// <summary>
    /// 商品售價
    /// ItemSellingPrice
    /// </summary>
    [DisplayName("ItemDetailsSellingPrice")]
    public int SellingPrice { get; set; }

    /// <summary>
    /// 商品團購價 Group Buy Price
    /// </summary>
    [DisplayName("ItemDetailsGroupBuyPrice")]
    public int? GroupBuyPrice { get; set; }

    /// <summary>
    /// 可販售數量限制
    /// </summary>
    [DisplayName("ItemDetailsSaleableQuantity")]
    public int? SaleableQuantity { get; set; }

    /// <summary>
    /// 可預購數量
    /// </summary>
    [DisplayName("ItemDetailsPreOrderableQuantity")]
    public int? PreOrderableQuantity { get; set; }

    /// <summary>
    /// 可訂購預購數量
    /// </summary>
    [DisplayName("ItemDetailsSaleablePreOrderQuantity")]
    public int? SaleablePreOrderQuantity { get; set; }

    /// <summary>
    /// SKU
    /// </summary>
    [DisplayName("ItemDetailsSKU")]
    public string SKU { get; set; }

    /// <summary>
    /// Reorder Level
    /// </summary>
    [DisplayName("ItemDetailsReorderLevel")]
    public int? ReorderLevel { get; set; }

    /// <summary>
    /// 倉庫名稱
    /// Warehouse Name
    /// </summary>
    [DisplayName("ItemDetailsWarehouseName")]
    public string? WarehouseName { get; set; }

    /// <summary>
    /// 庫存帳戶
    /// Inventory Account
    /// </summary>
    [DisplayName("ItemDetailsInventoryAccount")]
    public string? InventoryAccount { get; set; }

    /// <summary>
    /// 初始庫存
    /// Opening Stock
    /// </summary>
    [DisplayName("ItemDetailsOpeningStock")]
    public int? OpeningStock { get; set; }

    /// <summary>
    /// 初始庫存金額
    /// Opening Stock Value
    /// </summary>
    [DisplayName("ItemDetailsOpeningStockValue")]
    public int OpeningStockValue { get; set; }

    /// <summary>
    /// 採購金額
    /// Purchase Price
    /// </summary>
    [DisplayName("ItemDetailsPurchasePrice")]
    public int? PurchasePrice { get; set; }

    /// <summary>
    /// 採購帳戶
    ///  Purchase Account
    /// </summary>
    [DisplayName("ItemDetailsPurchaseAccount")]
    public string? PurchaseAccount { get; set; }

    /// <summary>
    /// 採購描述
    /// Purchase Description
    /// </summary>
    [DisplayName("ItemDetailsPurchaseDescription")]
    public string? PurchaseDescription { get; set; }

    /// <summary>
    /// 預設採購商
    /// Preferred Vendor
    /// </summary>
    [DisplayName("ItemDetailsPreferredVendor")]
    public string? PreferredVendor { get; set; }

    /// <summary>
    /// 現有庫存 - 這是統計全部倉庫的庫存
    /// Stock On Hand - This is the total stock of all warehouses
    /// </summary>
    [DisplayName("ItemDetailsStockOnHand")]
    public int? StockOnHand { get; set; }

    /// <summary>
    /// 部件編號
    ///  Part Number
    /// </summary>
    [DisplayName("ItemDetailsPartNumber")]
    public string? PartNumber { get; set; }

    /// <summary>
    /// 商品歸戶
    ///  Item Type
    /// </summary>
    [DisplayName("ItemDetailsItemType")]
    public string? ItemType { get; set; }
}