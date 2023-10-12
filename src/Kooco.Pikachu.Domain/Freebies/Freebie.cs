using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Freebies
{
    public class Freebie : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public bool ApplyToAllGroupBuy { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<FreebieGroupBuys> FreebieGroupBuys { get; set; }
        public bool UnCondition { get; set; }
        public DateTime? ActivityStartDate { get; set; }
        public DateTime? ActivityEndDate { get; set; }
        public FreebieOrderReach? FreebieOrderReach { get; set; }
        public decimal? MinimumAmount { get; set; }
        public int? MinimumPiece { get; set; }
        public int FreebieQuantity { get; set; }
        public decimal? FreebieAmount { get; set; }
        public Guid? TenantId { get; set; }
        public bool IsFreebieAvaliable { get; set; }

        public Freebie()
        {

        }
        public Freebie(
            [NotNull] Guid id,
            string itemName,
            string? itemDescription,
            bool applyToAllGroupBuy,
            bool unCondition,
            FreebieOrderReach? freebieOrderReach,
            DateTime? activityStartDate,
            DateTime? activityEndDate,
            decimal? minimumAmount,
            int? minimumPiece,
            int freebieQuantity,
            decimal? freebieAmount
            ) : base(id)
        {
            SetItemName(itemName);
            ItemDescription = itemDescription;
            ApplyToAllGroupBuy = applyToAllGroupBuy;
            UnCondition = unCondition;
            ActivityStartDate = activityStartDate;
            ActivityEndDate = activityEndDate;
            FreebieAmount = freebieAmount;
            MinimumAmount = minimumAmount;
            MinimumPiece = minimumPiece;
            FreebieQuantity = freebieQuantity;
            FreebieOrderReach = freebieOrderReach;
            Images = new List<Image>();
            FreebieGroupBuys = new List<FreebieGroupBuys>();
        }
        private void SetItemName(
          [NotNull] string itemName
          )
        {
            ItemName = Check.NotNullOrWhiteSpace(
                itemName,
                nameof(ItemName),
                maxLength: FreebieConsts.MaxItemNameLength
                );
        }
        public void AddFreebieGroupBuys(
                Guid freeBieId,
                Guid groupBuyId
        )
        {
            if (!FreebieGroupBuys.Any(x => x.GroupBuyId == groupBuyId))
            {
                FreebieGroupBuys.Add(
                    new FreebieGroupBuys(
                      freeBieId, groupBuyId
                        )
                    );
            }
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
        public void RemoveFreebieGroupBuys(List<Guid?> ids)
        {
            FreebieGroupBuys.RemoveAll(x => !ids.Contains(x.GroupBuyId));
        }

    }
}
