using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class Item : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public long ItemNo { get; set; } //商品編號/ItemNo
        public string ItemName { get; set; } //商品名稱/ItemName
        public ICollection<ItemDetails> ItemDetails { get; set; } //项目详情/ItemDetails
        public string? ItemDescriptionTitle { get; set; } //商品敘述抬頭/ItemDescriptionTitle
        public string? ItemDescription { get; set; }// 商品描述/ItemDescription
        public string ItemTags { get; set; } // 项目标签/ItemTags
        public string? SalesAccount { get; set; } //銷售帳戶/Sales Account
        public bool Returnable { get; set; } = false; // 可否退貨/Returnable
        public DateTime LimitAvaliableTimeStart { get; set; } //限時販售開始時間/Limit Avaliable Time Start
        public DateTime LimitAvaliableTimeEnd { get; set; } //限時販售結束時間/Limit Avaliable Time End
        public float ShareProfit { get; set; } //分潤/Share Profit
        public bool IsFreeShipping { get; set; } = false; //是否免運/Is Free Shipping
        public string? BrandName { get; set; } //商品品牌名稱/Item Brand Name
        public string? ManufactorName { get; set; } //商品製造商名稱/Item Manufactor Name
        public float? PackageWeight { get; set; } //包裝寬度/Package Weight
        public float? PackageLength { get; set; } //包裝深度/PackageLength
        public float? PackageHeight { get; set; } //包裝高度/Package Height
        public Diemensions? DiemensionsUnit { get; set; } //度量單位/Dimension Unit
        public Weight? WeightUnit { get; set; } //重量單位/Weight Unit
        public EnumValue? TaxType { get; set; } //商品稅別/Tax Type
        public EnumValue? Unit { get; set; } //商品單位/Unit
        public EnumValue? ShippingMethod { get; set; } 
        public bool IsReturnable { get; set; }
        public bool IsItemAvaliable { get; set; }
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






        public int? TaxPercentage { get; set; } //稅率百分比/Tax Percentage
        public string? TaxName { get; set; } //稅率名稱/Tax Name
        public string? PurchaseTaxName { get; set; } //採購稅率名稱/Purchase Tax Name
        public int? PurchaseTaxPercentage { get; set; } //採購稅率百分比/Purchase Tax Percentage
        public string? ProductType { get; set; } //商品類型/Product Type
        public int? Source { get; set; } //商品來源/Source
        public int? ReferenceID { get; set; } //参考编号/Reference ID
        public DateTime? LastSyncTime { get; set; }  //最後同步時間/Last Sync Time
        public string? Status { get; set; } //商品狀態/Status
        public string? UPC { get; set; }
        public string? EAN { get; set; }
        public string? ISBN { get; set; }
        public string? ItemCategory { get; set; }
        public String? ItemMainImageURL { get; set; } //商品類別/Item Category
    }
}
