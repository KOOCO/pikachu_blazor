using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;



namespace Kooco.Pikachu.Items.Dtos;

/// <summary>
/// 
/// </summary>
[Serializable]
public class ItemDto : FullAuditedEntityDto<Guid>
{
    public string ItemName { get; set; } //商品名稱/ItemName
    public ICollection<ItemDetailsDto> ItemDetails { get; set; }
    public String? ItemMainImageURL { get; set; }
    public string ItemDescriptionTitle { get; set; } //商品敘述抬頭/ItemDescriptionTitle
    public string ItemDescription { get; set; } //商品描述/ItemDescription
    public Boolean Returnable { get; set; } //可否退貨/Returnable
    public DateTime LimitAvaliableTimeStart { get; set; } //限時販售開始時間/Limit Avaliable Time Start
    public DateTime LimitAvaliableTimeEnd { get; set; } //限時販售結束時間/Limit Avaliable Time End
    public int QuantityLimit { get; set; } //販售數量限制/QuantityLimit
    public int ShareProfit { get; set; } // 分潤/Share Profit
    public bool IsFreeShipping { get; set; } = false; //是否免運/Is Free Shipping

    //todo add shipping method
    /// <summary>
    /// 排除運送方式 Exclude Shipping Method
    /// </summary>
    //public ICollection<string> ExclueShippingMethod { get; set; }


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