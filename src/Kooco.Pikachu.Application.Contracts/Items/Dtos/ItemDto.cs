using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.ProductCategories;
using Kooco.Pikachu.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class ItemDto : FullAuditedEntityDto<Guid>
{
    public string ItemName { get; set; } //商品名稱/ItemName
    
    /// <summary>
    /// 商品編號
    /// </summary>
    public long ItemNo { get; set; }
    [MaxLength(4)]
    public string ItemBadge { get; set; } //商品名稱/ItemName
    public string? ItemBadgeColor { get; set; }
    public string ItemDescriptionTitle { get; set; }
    public string? ItemDescription { get; set; }
    public DateTime LimitAvaliableTimeStart { get; set; }
    public DateTime LimitAvaliableTimeEnd { get; set; }
    public int LimitQuantity { get; set; }
    public int ShareProfit { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool IsReturnable { get; set; }
    public bool IsItemAvaliable { get; set; }
    public bool IsSelected { get; set; } = false;
    public string ItemTags { get; set; }
    public int ShippingMethodId { get; set; }
    public int TaxTypeId { get; set; }

    public string? Attribute1Name { get; set; }
    public string? Attribute2Name { get; set; }
    public string? Attribute3Name { get; set; }

    public ItemStorageTemperature ItemStorageTemperature { get; set; }

    public virtual ICollection<ItemDetailsDto> ItemDetails { get; set; }
    public virtual ICollection<ImageDto> Images { get; set; }
    public List<CategoryProductDto> CategoryProducts { get; set; } = [];
    public List<AttributeNameOption> AttributeNameOptions { get; set; }
}