using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class SetItem : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public ICollection<SetItemDetails> SetItemDetails { get; set; }
        public ICollection<Image> Images { get; set; }
        public string SetItemName { get; set; }
        public string? SetItemNo { get; set; }
        public string? SetItemDescriptionTitle { get; set; }
        public string? Description { get; set; }
        public string? SetItemMainImageURL { get; set; }

        public string? SetItemStatus { get; set; }
        public int? SetItemSaleableQuantity { get; set; }

        public int SellingPrice { get; set; }
        public int? GroupBuyPrice { get; set; }

        /// <summary>
        /// 可販售數量限制
        /// </summary>
        public float SetItemPrice { get; set; }
        public int? LimitQuantity { get; set; }
        public int? SaleableQuantity { get; set; }

        /// <summary>
        /// 可預購數量
        /// </summary>
        public int? PreOrderableQuantity { get; set; }

        /// <summary>
        /// 可訂購預購數量
        /// </summary>
        public int? SaleablePreOrderQuantity { get; set; }

        //todo add item tag
        //public Array ItemTags { get; set; }
        /// <summary>
        /// 銷售帳戶
        /// Sales Account
        /// </summary>
        public string? SalesAccount { get; set; }
        /// <summary>
        /// 可否退貨
        /// Returnable
        /// </summary>
        public Boolean Returnable { get; set; } = false;
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
        public bool IsFreeShipping { get; set; } = false;

        //todo add shipping method
        /// <summary>
        /// 排除運送方式 Exclude Shipping Method
        /// </summary>
        //public ICollection<string> ExclueShippingMethod { get; set; }
        /// <summary>
        /// 稅率名稱
        /// Tax Name
        /// </summary>
        public string? TaxName { get; set; }
        /// <summary>
        /// 稅率百分比
        /// Tax Percentage
        /// </summary>
        public int? TaxPercentage { get; set; }
        /// <summary>
        /// 商品稅別
        /// Tax Type
        /// </summary>
        public string? TaxType { get; set; }

        /// <summary>
        /// 商品類別
        /// Item Category
        /// </summary>
        public string? ItemCategory { get; set; }

        public ItemStorageTemperature? ItemStorageTemperature { get; set; }

        public SetItem()
        {
        }

        public SetItem(
            Guid id,
            Guid? tenantId,
            string setItemName,
            string? setItemDescriptionTitle,
            string? description,
            string? setItemMainImageURL,
            float setItemPrice,
            int? limitQuantity,
            DateTime limitAvaliableTimeStart,
            DateTime limitAvaliableTimeEnd,
            int shareProfit,
            bool isFreeShipping,
            ItemStorageTemperature? itemStorageTemperature
        ) : base(id)
        {
            TenantId = tenantId;
            SetItemName = setItemName;
            SetItemDescriptionTitle = setItemDescriptionTitle;
            Description = description;
            SetItemMainImageURL = setItemMainImageURL;
            SetItemPrice = setItemPrice;
            LimitQuantity = limitQuantity;
            LimitAvaliableTimeStart = limitAvaliableTimeStart;
            LimitAvaliableTimeEnd = limitAvaliableTimeEnd;
            ShareProfit = shareProfit;
            IsFreeShipping = isFreeShipping;
            ItemStorageTemperature = itemStorageTemperature;
            SetItemDetails = new List<SetItemDetails>();
            Images = new List<Image>();
        }


        public void AddSetItemDetails(
        Guid id,
        Guid? tenantId,
        Guid setItemId,
        Guid itemId,
        int quantity,
        string? attribute1Value,
        string? attribute2Value,
        string? attribute3Value
        )
        {
            SetItemDetails.Add(
                new SetItemDetails(
                    id,
                    tenantId,
                    setItemId,
                    itemId,
                    quantity,
                    attribute1Value,
                    attribute2Value,
                    attribute3Value
                    )
                );
        }

        public void AddImage(
            Guid id,
            string name,
            string blobImageName,
            string imageUrl,
            int sortNo
            )
        {
            Images.Add(new Image(id, name, blobImageName, imageUrl, ImageType.SetItem, this.Id, sortNo));
        }

        public void RemoveItemDetailsAsync(List<Guid?> ids)
        {
            SetItemDetails.RemoveAll(x => !ids.Contains(x.Id));
        }
    }
}
