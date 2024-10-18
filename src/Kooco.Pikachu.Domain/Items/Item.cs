using JetBrains.Annotations;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.ProductCategories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class Item : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public long ItemNo { get; set; } //商品編號/ItemNo
        public string ItemName { get; set; } //商品名稱/ItemName
        public virtual ICollection<ItemDetails> ItemDetails { get; set; } //项目详情/ItemDetails
        public virtual ICollection<Image> Images { get; set; }
        public string? ItemDescriptionTitle { get; set; } //商品敘述抬頭/ItemDescriptionTitle
        public string? ItemDescription { get; set; }// 商品描述/ItemDescription
        public string? ItemTags { get; set; } // 项目标签/ItemTags
        public string? SalesAccount { get; set; } //銷售帳戶/Sales Account
        public bool Returnable { get; set; } = false; // 可否退貨/Returnable
        public DateTime? LimitAvaliableTimeStart { get; set; } //限時販售開始時間/Limit Avaliable Time Start
        public DateTime? LimitAvaliableTimeEnd { get; set; } //限時販售結束時間/Limit Avaliable Time End
        public float ShareProfit { get; set; } //分潤/Share Profit
        public bool IsFreeShipping { get; set; } = false; //是否免運/Is Free Shipping
        public string? BrandName { get; set; } //商品品牌名稱/Item Brand Name
        public string? ManufactorName { get; set; } //商品製造商名稱/Item Manufactor Name
        public float? PackageWeight { get; set; } //包裝寬度/Package Weight
        public float? PackageLength { get; set; } //包裝深度/PackageLength
        public float? PackageHeight { get; set; } //包裝高度/Package Height
        public Diemensions? DiemensionsUnit { get; set; } //度量單位/Dimension Unit
        public Weight? WeightUnit { get; set; } //重量單位/Weight Unit
        public virtual EnumValue? TaxType { get; set; } //商品稅別/Tax Type
        public int? TaxTypeId { get; set; } //商品稅別/Tax Type
        public virtual EnumValue? Unit { get; set; } //商品單位/Unit
        public int? UnitId { get; set; } //商品單位/Unit
        public virtual EnumValue? ShippingMethod { get; set; }
        public int? ShippingMethodId { get; set; }
        public bool IsReturnable { get; set; }
        public bool IsItemAvaliable { get; set; }
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

        public string? Attribute1Name { get; set; }
        public string? Attribute2Name { get; set; }
        public string? Attribute3Name { get; set; }

        public ItemStorageTemperature? ItemStorageTemperature { get; set; }
        public List<CategoryProduct> CategoryProducts { get; set; }

        public Item() { }
        public Item(
            Guid id,
            [NotNull] string itemName,
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
            float saleableQuantity,
            float? preOrderableQuantity,
            float? saleablePreOrderQuantity,
            float? groupBuyPrice,
            string? inventoryAccount,
            string? attribute1Value,
            string? attribute2Value,
            string? attribute3Value,
            string? image,
            string? itemDescription,
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
                    saleableQuantity,
                    groupBuyPrice,
                    preOrderableQuantity,
                    saleablePreOrderQuantity,
                    inventoryAccount,
                    attribute1Value,
                    attribute2Value,
                    attribute3Value,
                    image,
                    itemDescription,
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
}
