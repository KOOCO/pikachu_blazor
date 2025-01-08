using System;
using Volo.Abp.Application.Dtos;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Items.Dtos;

/// <summary>
/// 
/// </summary>
[Serializable]
public class ItemDetailsDto : FullAuditedEntityDto<Guid>
{
    public Guid ItemId { get; set; }
    public ItemDto Item { get; set; }
    public string ItemName { get; set; }
    public string? ItemDetailTitle { get; set; }
    public string? ItemDetailStatus { get; set; }
    public string? ItemDetailDescription { get; set; }
    public string? Property1 { get; set; }
    public string? Property2 { get; set; }
    public string? Property3 { get; set; }
    public int? LimitQuantity { get; set; }
    public string? Attribute1Value { get; set; }
    public string? Attribute2Value { get; set; }
    public string? Attribute3Value { get; set; }
    public string? ItemDescription { get; set; }
    public string? Image { get; set; }
    public bool Status { get; set; }
    public float Cost { get; set; }
    /// <summary>
    /// 商品售價
    ///ItemSellingPrice
    /// </summary>
    public int SellingPrice { get; set; }

    /// <summary>
    /// 商品團購價 Group Buy Price
    /// </summary>
    public int? GroupBuyPrice { get; set; }

    /// <summary>
    /// 可販售數量限制
    /// </summary>
    public int? SaleableQuantity { get; set; }

    /// <summary>
    /// 可預購數量
    /// </summary>
    public int? PreOrderableQuantity { get; set; }

    /// <summary>
    /// 可訂購預購數量
    /// </summary>
    public int? SaleablePreOrderQuantity { get; set; }

    /// <summary>
    /// SKU
    /// </summary>
    public string SKU { get; set; }

    /// <summary>
    /// Reorder Level
    /// </summary>
    public int? ReorderLevel { get; set; }

    /// <summary>
    /// 倉庫名稱
    ///  Warehouse Name
    /// </summary>
    public string? WarehouseName { get; set; }

    /// <summary>
    /// 庫存帳戶
    ///  Inventory Account
    /// </summary>
    public string? InventoryAccount { get; set; }

    /// <summary>
    /// 初始庫存
    ///  Opening Stock
    /// </summary>
    public int? OpeningStock { get; set; }

    /// <summary>
    /// 初始庫存金額
    /// Opening Stock Value
    /// </summary>
    public int OpeningStockValue { get; set; }

    /// <summary>
    /// 採購金額
    ///  Purchase Price
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
    /// 預設採購商
    /// Preferred Vendor
    /// </summary>
    public string? PreferredVendor { get; set; }

    /// <summary>
    /// 現有庫存 - 這是統計全部倉庫的庫存
    /// Stock On Hand - This is the total stock of all warehouses
    /// </summary>
    public int? StockOnHand { get; set; }

    /// <summary>
    /// 部件編號
    ///  Part Number
    /// </summary>
    public string? PartNumber { get; set; }

    /// <summary>
    /// 商品歸戶
    /// Item Type
    /// </summary>
    public string? ItemType { get; set; }
}