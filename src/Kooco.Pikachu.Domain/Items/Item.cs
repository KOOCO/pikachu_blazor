using JetBrains.Annotations;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.ProductCategories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items;
public class Item : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租戶 ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public long ItemNo { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ItemName { get; set; }

    /// <summary>
    /// 商品標記 (例如：新品、熱銷)
    /// </summary>
    [MaxLength(4)]
    public string? ItemBadge { get; set; }

    /// <summary>
    /// 商品敘述抬頭，用於描述商品的標題。
    /// </summary>
    public string? ItemDescriptionTitle { get; set; }

    /// <summary>
    /// 商品描述，詳細描述商品的特性或功能。
    /// </summary>
    public string? ItemDescription { get; set; }

    /// <summary>
    /// 項目標籤，用於分類或標記商品。
    /// </summary>
    public string? ItemTags { get; set; }

    /// <summary>
    /// 銷售帳戶，記錄負責銷售該商品的帳戶。
    /// </summary>
    public string? SalesAccount { get; set; }

    /// <summary>
    /// 是否可退貨，預設為false。
    /// </summary>
    public bool Returnable { get; set; } = false;

    /// <summary>
    /// 限時販售的開始時間。
    /// </summary>
    public DateTime? LimitAvaliableTimeStart { get; set; }

    /// <summary>
    /// 限時販售的結束時間。
    /// </summary>
    public DateTime? LimitAvaliableTimeEnd { get; set; }

    /// <summary>
    /// 分潤比例，表示銷售該商品所獲得的利潤分成。
    /// </summary>
    public float ShareProfit { get; set; }

    /// <summary>
    /// 是否免運，預設為false。
    /// </summary>
    public bool IsFreeShipping { get; set; } = false;

    /// <summary>
    /// 商品品牌名稱。
    /// </summary>
    public string? BrandName { get; set; }

    /// <summary>
    /// 商品製造商名稱。
    /// </summary>
    public string? ManufactorName { get; set; }

    /// <summary>
    /// 包裝的重量。
    /// </summary>
    public float? PackageWeight { get; set; }

    /// <summary>
    /// 包裝的長度。
    /// </summary>
    public float? PackageLength { get; set; }

    /// <summary>
    /// 包裝的高度。
    /// </summary>
    public float? PackageHeight { get; set; }

    /// <summary>
    /// 度量單位。
    /// </summary>
    public Diemensions? DiemensionsUnit { get; set; }

    /// <summary>
    /// 重量單位。
    /// </summary>
    public Weight? WeightUnit { get; set; }

    /// <summary>
    /// 商品稅別。
    /// </summary>
    public virtual EnumValue? TaxType { get; set; }

    /// <summary>
    /// 商品稅別ID。
    /// </summary>
    public int? TaxTypeId { get; set; }

    /// <summary>
    /// 商品單位。
    /// </summary>
    public virtual EnumValue? Unit { get; set; }

    /// <summary>
    /// 商品單位ID。
    /// </summary>
    public int? UnitId { get; set; }

    /// <summary>
    /// 運輸方式。
    /// </summary>
    public virtual EnumValue? ShippingMethod { get; set; }

    /// <summary>
    /// 運輸方式ID。
    /// </summary>
    public int? ShippingMethodId { get; set; }

    /// <summary>
    /// 是否可退貨。
    /// </summary>
    public bool IsReturnable { get; set; }

    /// <summary>
    /// 商品是否可用。
    /// </summary>
    public bool IsItemAvaliable { get; set; }

    /// <summary>
    /// 自定義欄位名稱和值，用於擴展商品的屬性。
    /// </summary>
    public string? CustomField1Name { get; set; }
    public string? CustomField1Value { get; set; }
    public string? CustomField2Name { get; set; }
    public string? CustomField2Value { get; set; }
    public string? CustomField3Name { get; set; }
    public string? CustomField3Value { get; set; }
    public string? CustomField4Name { get; set; }
    public string? CustomField4Value { get; set; }
    public string? CustomField5Name { get; set; }
    public string? CustomField5Value { get; set; }
    public string? CustomField6Name { get; set; }
    public string? CustomField6Value { get; set; }
    public string? CustomField7Name { get; set; }
    public string? CustomField7Value { get; set; }
    public string? CustomField8Name { get; set; }
    public string? CustomField8Value { get; set; }
    public string? CustomField9Name { get; set; }
    public string? CustomField9Value { get; set; }
    public string? CustomField10Name { get; set; }
    public string? CustomField10Value { get; set; }

    /// <summary>
    /// 稅率百分比。
    /// </summary>
    public int? TaxPercentage { get; set; }

    /// <summary>
    /// 稅率名稱。
    /// </summary>
    public string? TaxName { get; set; }

    /// <summary>
    /// 採購稅率名稱。
    /// </summary>
    public string? PurchaseTaxName { get; set; }

    /// <summary>
    /// 採購稅率百分比。
    /// </summary>
    public int? PurchaseTaxPercentage { get; set; }

    /// <summary>
    /// 商品類型。
    /// </summary>
    public string? ProductType { get; set; }

    /// <summary>
    /// 商品來源。
    /// </summary>
    public int? Source { get; set; }

    /// <summary>
    /// 參考編號。
    /// </summary>
    public int? ReferenceID { get; set; }

    /// <summary>
    /// 最後同步時間。
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// 商品狀態。
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 通用商品代碼 (UPC)。
    /// </summary>
    public string? UPC { get; set; }

    /// <summary>
    /// 國際商品編號 (EAN)。
    /// </summary>
    public string? EAN { get; set; }

    /// <summary>
    /// 國際標準書號 (ISBN)。
    /// </summary>
    public string? ISBN { get; set; }

    /// <summary>
    /// 商品類別。
    /// </summary>
    public string? ItemCategory { get; set; }

    /// <summary>
    /// 商品主圖URL。
    /// </summary>
    public string? ItemMainImageURL { get; set; }

    /// <summary>
    /// 自定義屬性名稱，用於擴展商品的屬性。
    /// </summary>
    public string? Attribute1Name { get; set; }
    public string? Attribute2Name { get; set; }
    public string? Attribute3Name { get; set; }

    public ItemStorageTemperature? ItemStorageTemperature { get; set; }
    public List<CategoryProduct> CategoryProducts { get; set; }

    public virtual ICollection<ItemDetails> ItemDetails { get; set; }
    public virtual ICollection<Image> Images { get; set; }

    public Item() { }
    public Item(
        Guid id,
        [NotNull] string itemName,
        string? itemBadge,
        string? itemDescriptionTitle,
        string? itemDescription,
        string? itemTags,
        DateTime? limitAvailableTimeStart,
        DateTime? limitAvailableTimeEnd,
        float shareProfit,
        bool isFreeShipping,
        bool isReturnable,
        int? shippingMethodId,
        int? taxTypeId,

        string? customField1Value,
        string? customField1Name,

        string? customField2Value,
        string? customField2Name,
        string? customField3Value,
        string? customField3Name,
        string? customField4Value,
        string? customField4Name,
        string? customField5Value,
        string? customField5Name,
        string? customField6Value,
        string? customField6Name,
        string? customField7Value,
        string? customField7Name,
        string? customField8Value,
        string? customField8Name,
        string? customField9Value,
        string? customField9Name,
        string? customField10Value,
        string? customField10Name,

        string? attribute1Name,
        string? attribute2Name,
        string? attribute3Name,

        ItemStorageTemperature? itemTemperature

        ) : base(id)
    {
        SetItemName(itemName);
        SetItemDescriptionTitle(itemDescriptionTitle);
        ItemDescription = itemDescription;
        ItemTags = itemTags;
        LimitAvaliableTimeStart = limitAvailableTimeStart;
        LimitAvaliableTimeEnd = limitAvailableTimeEnd;
        ShareProfit = shareProfit;
        IsFreeShipping = isFreeShipping;
        IsReturnable = isReturnable;
        ShippingMethodId = shippingMethodId;
        TaxTypeId = taxTypeId;
        SetItemBadge(itemBadge);
        CustomField1Value = customField1Value;
        CustomField1Name = customField1Name;
        CustomField2Value = customField2Value;
        CustomField2Name = customField2Name;
        CustomField3Value = customField3Value;
        CustomField3Name = customField3Name;
        CustomField4Value = customField4Value;
        CustomField4Name = customField4Name;
        CustomField5Value = customField5Value;
        CustomField5Name = customField5Name;
        CustomField6Value = customField6Value;
        CustomField6Name = customField6Name;
        CustomField7Value = customField7Value;
        CustomField7Name = customField7Name;
        CustomField8Value = customField8Value;
        CustomField8Name = customField8Name;
        CustomField9Value = customField9Value;
        CustomField9Name = customField9Name;
        CustomField10Value = customField10Value;
        CustomField10Name = customField10Name;

        Attribute1Name = attribute1Name;
        Attribute2Name = attribute2Name;
        Attribute3Name = attribute3Name;

        ItemStorageTemperature = itemTemperature;

        ItemDetails = new Collection<ItemDetails>();
        Images = new Collection<Image>();
        CategoryProducts = [];
    }

    private void SetItemName(
        [NotNull] string itemName
        )
    {
        ItemName = Check.NotNullOrWhiteSpace(
            itemName,
            nameof(ItemName),
            maxLength: ItemConsts.MaxItemNameLength
            );
    }
    private void SetItemBadge(
    [CanBeNull] string? itemBadge
    )
    {
        ItemBadge = Check.Length(
         itemBadge,
         nameof(ItemBadge),
         ItemConsts.MaxItemBadgeLength
         );
    }
    private void SetItemDescriptionTitle(
        [CanBeNull] string? itemDescriptionTitle
        )
    {
        ItemDescriptionTitle = Check.Length(
            itemDescriptionTitle,
            nameof(ItemDescriptionTitle),
            ItemConsts.MaxDescriptionTitleLength
            );
    }

    internal Item AddItemDetail(
        [NotNull] Guid id,
        [NotNull] string itemName,
        [NotNull] string sku,
        int? limitQuantity,
        float sellingPrice,
        float cost,
        float saleableQuantity,
        int? stockonHand,
        float? preOrderableQuantity,
        float? saleablePreOrderQuantity,

        string? inventoryAccount,
        string? attribute1Value,
        string? attribute2Value,
        string? attribute3Value,
        string? image,
        string? itemDescription,
        int sortNo,
        bool status = false
    )
    {
        if (ItemDetails.Any(x => x.SKU == sku))
        {
            throw new BusinessException(PikachuDomainErrorCodes.ItemWithSKUAlreadyExists)
                .WithData("SKU", sku);
        }

        ItemDetails.Add(
            new ItemDetails(
                id,
                itemName,
                sku,
                Id,
                limitQuantity,
                sellingPrice,
                cost,
                saleableQuantity,
                stockonHand,

                preOrderableQuantity,
                saleablePreOrderQuantity,
                inventoryAccount,
                attribute1Value,
                attribute2Value,
                attribute3Value,
                image,
                itemDescription,
                sortNo,
                status
            )
        );

        return this;
    }

    internal Item AddItemImage(
        [NotNull] Guid id,
        string name,
        string blobImageName,
        string imageUrl,
        ImageType imageType,
        Guid? targetId,
        int sortNo
        )
    {
        Images.Add(
            new Image(
                id,
                name,
                blobImageName,
                imageUrl,
                imageType,
                targetId,
                sortNo
                )
            );
        return this;
    }

    public CategoryProduct AddCategoryProduct(Guid categoryId)
    {
        var categoryProduct = new CategoryProduct(Id, categoryId);
        CategoryProducts ??= [];
        CategoryProducts.AddIfNotContains(categoryProduct);
        return categoryProduct;
    }
}
