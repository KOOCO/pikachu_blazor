﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public decimal? FreebieAmount { get; set; }
        public Guid? TenantId { get; set; }

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
            FreebieGroupBuys.Add(
                new FreebieGroupBuys(
                  freeBieId, groupBuyId
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


    }
}