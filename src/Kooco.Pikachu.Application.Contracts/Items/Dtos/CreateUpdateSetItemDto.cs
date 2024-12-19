using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateUpdateSetItemDto
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSetItemDetails")]
    public List<CreateUpdateSetItemDetailsDto> SetItemDetails { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSetItemName")]
    public string SetItemName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSetItemNo")]
    public string? SetItemNo { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSetItemDescriptionTitle")]
    public string? SetItemDescriptionTitle { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemDescription")]
    public string? Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSetItemMainImageURL")]
    public string? SetItemMainImageURL { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSetItemStatus")]
    public string SetItemStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSetItemSaleableQuantity")]
    public int? SetItemSaleableQuantity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemSellingPrice")]
    public int SellingPrice { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemGroupBuyPrice")]
    public int? GroupBuyPrice { get; set; }

    [DisplayName("SetItemPrice")]
    public float SetItemPrice { get; set; }

    [DisplayName("LimitQuantity")]
    public int? LimitQuantity { get; set; }

    /// <summary>
    /// 可販售數量限制
    /// </summary>
    [DisplayName("SetItemSaleableQuantity")]
    public int? SaleableQuantity { get; set; }

    /// <summary>
    /// 可預購數量
    /// </summary>
    [DisplayName("SetItemPreOrderableQuantity")]
    public int? PreOrderableQuantity { get; set; }

    /// <summary>
    /// 可訂購預購數量
    /// </summary>
    [DisplayName("SetItemSaleablePreOrderQuantity")]
    public int? SaleablePreOrderQuantity { get; set; }

    /// <summary>
    /// 銷售帳戶        Sales Account
    /// </summary>
    [DisplayName("SetItemSalesAccount")]
    public string? SalesAccount { get; set; }

    /// <summary>
    /// 可否退貨        Returnable
    /// </summary>
    [DisplayName("SetItemReturnable")]
    public Boolean Returnable { get; set; }

    /// <summary>
    /// 限時販售開始時間 Ｌimit Avaliable Time Start
    /// </summary>
    [DisplayName("SetItemLimitAvaliableTimeStart")]
    public DateTime LimitAvaliableTimeStart { get; set; } = DateTime.Now;

    /// <summary>
    /// 限時販售結束時間 Ｌimit Avaliable Time End
    /// </summary>
    [DisplayName("SetItemLimitAvaliableTimeEnd")]
    public DateTime LimitAvaliableTimeEnd { get; set; } = DateTime.Now;

    /// <summary>
    /// 分潤 Share Profit
    /// </summary>
    [DisplayName("SetItemShareProfit")]
    public int ShareProfit { get; set; }

    /// <summary>
    /// 是否免運 Is Free Shipping
    /// </summary>
    [DisplayName("SetItemisFreeShipping")]
    public bool IsFreeShipping { get; set; }

    /// <summary>
    /// 排除運送方式 Exclude Shipping Method;稅率名稱        Tax Name
    /// </summary>
    [DisplayName("SetItemTaxName")]
    public string? TaxName { get; set; }

    /// <summary>
    /// 稅率百分比        Tax Percentage
    /// </summary>
    [DisplayName("SetItemTaxPercentage")]
    public int? TaxPercentage { get; set; }

    /// <summary>
    /// 商品稅別        Tax Type
    /// </summary>
    [DisplayName("SetItemTaxType")]
    public string? TaxType { get; set; }

    /// <summary>
    /// 商品類別        Item Category
    /// </summary>
    [DisplayName("SetItemItemCategory")]
    public string? ItemCategory { get; set; }

    public List<CreateImageDto> Images { get; set; } = new();

    public ItemStorageTemperature? ItemStorageTemperature { get; set; }
}