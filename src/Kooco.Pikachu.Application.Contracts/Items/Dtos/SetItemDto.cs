using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Items.Dtos;

/// <summary>
/// 
/// </summary>
[Serializable]
public class SetItemDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 
    /// </summary>
    public ICollection<SetItemDetailsDto> SetItemDetails { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string SetItemName { get; set; }
    public string? SetItemBadge { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? SetItemNo { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? SetItemDescriptionTitle { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? SetItemMainImageURL { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string SetItemStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? SetItemSaleableQuantity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int SellingPrice { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public float? GroupBuyPrice { get; set; }
    public float SetItemPrice { get; set; }
    public int? LimitQuantity { get; set; }

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
    /// 銷售帳戶        Sales Account
    /// </summary>
    public string? SalesAccount { get; set; }

    /// <summary>
    /// 可否退貨        Returnable
    /// </summary>
    public Boolean Returnable { get; set; }

    /// <summary>
    /// 限時販售開始時間 Ｌimit Avaliable Time Start
    /// </summary>
    public DateTime LimitAvaliableTimeStart { get; set; }

    /// <summary>
    /// 限時販售結束時間 Ｌimit Avaliable Time End
    /// </summary>
    public DateTime LimitAvaliableTimeEnd { get; set; }

    /// <summary>
    /// 分潤 Share Profit
    /// </summary>
    public int ShareProfit { get; set; }

    /// <summary>
    /// 是否免運 Is Free Shipping
    /// </summary>
    public bool IsFreeShipping { get; set; }

    /// <summary>
    /// 排除運送方式 Exclude Shipping Method;稅率名稱        Tax Name
    /// </summary>
    public string? TaxName { get; set; }

    /// <summary>
    /// 稅率百分比        Tax Percentage
    /// </summary>
    public int? TaxPercentage { get; set; }

    /// <summary>
    /// 商品稅別        Tax Type
    /// </summary>
    public string? TaxType { get; set; }

    /// <summary>
    /// 商品類別        Item Category
    /// </summary>
    public string? ItemCategory { get; set; }

    public bool IsSelected { get; set; } = false;

    public ICollection<ImageDto> Images { get; set; }
    public ItemStorageTemperature? ItemStorageTemperature { get; set; }
}