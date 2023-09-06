using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class ItemDetails : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid ItemId { get; set; }
        public virtual Item Item { get; set; }
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public float SellingPrice { get; set; } //商品售價/ItemSellingPrice
        public float? GroupBuyPrice { get; set; } //商品團購價/Group Buy Price
        public float? SaleableQuantity { get; set; } //可販售數量限制/SaleableQuantity
        public float? PreOrderableQuantity { get; set; } //可預購數量/PreOrderableQuantity
        public float? SaleablePreOrderQuantity { get; set; } //可訂購預購數量/SaleablePreOrderQuantity
        public int? LimitQuantity { get; set; }



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

        public string? Attribute1Value { get; set; }
        public string? Attribute2Value { get; set; }
        public string? Attribute3Value { get; set; }

        public ItemDetails() { }

        public ItemDetails(
            [NotNull] Guid id,
            [NotNull] string itemName,
            [NotNull] string sku,
            Guid itemId,
            int? limitQuantity,
            float sellingPrice,
            float saleableQuantity,
            float? preOrderableQuantity,
            float? saleablePreOrderQuantity,
            string? inventoryAccount,

            string? attribute1Value,
            string? attribute2Value,
            string? attribute3Value
            ) : base(id)
        {
            SetSKU(sku);
            SetItemName(itemName);

            ItemId = itemId;
            ItemName = itemName;
            LimitQuantity = limitQuantity;
            SellingPrice = sellingPrice;
            SaleableQuantity = saleableQuantity;
            PreOrderableQuantity = preOrderableQuantity;
            SaleablePreOrderQuantity = saleablePreOrderQuantity;
            InventoryAccount = inventoryAccount;

            Attribute1Value = attribute1Value;
            Attribute2Value = attribute2Value;
            Attribute3Value = attribute3Value;
        }

        private void SetSKU(
            [NotNull] string sku
            )
        {
            SKU = Check.NotNull(sku, nameof(SKU));
        }

        private void SetItemName(
            [NotNull] string itemName
            )
        {
            ItemName = Check.NotNull(itemName, nameof(ItemName));
        }
    }
}
