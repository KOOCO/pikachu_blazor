using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class ItemDetails : FullAuditedAggregateRoot<Guid>,IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public float SellingPrice { get; set; } //商品售價/ItemSellingPrice
        public float? GroupBuyPrice { get; set; } //商品團購價/Group Buy Price
        public float? SaleableQuantity { get; set; } //可販售數量限制/SaleableQuantity
        public float? PreOrderableQuantity { get; set; } //可預購數量/PreOrderableQuantity
        public float? SaleablePreOrderQuantity { get; set; } //可訂購預購數量/SaleablePreOrderQuantity




        public int? ReorderLevel { get; set; }
        public string? WarehouseName { get; set; } //倉庫名稱/Warehouse Name
        public string? InventoryAccount { get; set; } //庫存帳戶/Inventory Account
        public int? OpeningStock { get; set; } //初始庫存/Opening Stock
        public int OpeningStockValue { get; set; } //初始庫存金額/Opening Stock Value
        public int? PurchasePrice { get; set; } //採購金額/Purchase Price
        public string? PurchaseAccount { get; set; } //採購帳戶/Purchase Account
        public string? PurchaseDescription { get; set; } //採購描述/Purchase Description
        public string? PreferredVendor { get; set; } //預設採購商 /Preferred Vendor
        public int? StockOnHand { get; set; } = 0; // 現有庫存 - 這是統計全部倉庫的庫存 /Stock On Hand - This is the total stock of all warehouses
        public string? PartNumber { get; set; } //部件編號/Part Number
        public string? ItemType { get; set; } //Item Type/商品歸戶
        public string? ItemDetailTitle { get; set; }
        public string? ItemDetailStatus { get; set; }
        public string? ItemDetailDescription { get; set; }
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
        public string? Property3 { get; set; }
    }
}
