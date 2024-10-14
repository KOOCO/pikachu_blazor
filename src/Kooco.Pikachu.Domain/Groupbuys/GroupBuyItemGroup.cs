using System;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities;
using Kooco.Pikachu.Groupbuys;
using System.Collections.Generic;
using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.GroupBuys
{ 
    public class GroupBuyItemGroup : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 團購編號
        /// </summary>
        public Guid GroupBuyId { get; set; }

        /// <summary>
        /// 商品組合排序 Sorting Orger of groupbuy item groupf
        /// </summary>
        public int SortOrder { get; set; }

        public GroupBuyModuleType GroupBuyModuleType { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? ProductGroupModuleTitle { get; set; }
        public string? ProductGroupModuleImageSize { get; set; }

        public ICollection<GroupBuyItemGroupDetails> ItemGroupDetails { get; set; }

        public GroupBuyItemGroup()
        {
            
        }

        public GroupBuyItemGroup(
            Guid id,
            Guid groupBuyId,
            int sortOrder,
            GroupBuyModuleType groupBuyModuleType,
            string? additionalInfo,
            string? productGroupModuleTitle,
            string? productGroupModuleImageSize
        ) : base(id)
        {
            GroupBuyId = groupBuyId;
            SortOrder = sortOrder;
            GroupBuyModuleType = groupBuyModuleType;
            AdditionalInfo = additionalInfo;
            ProductGroupModuleTitle = productGroupModuleTitle;
            ProductGroupModuleImageSize = productGroupModuleImageSize;
            ItemGroupDetails = new List<GroupBuyItemGroupDetails>();
        }

        public void GroupBuyItemGroupDetails(
            Guid id,
            Guid groupBuyItemGroupId,
            int sortOrder,
            Guid? itemId,
            Guid? setItemId,
            ItemType itemType,
            string? displayText
            )
        {
            ItemGroupDetails.Add(new GroupBuyItemGroupDetails(
                id,
                groupBuyItemGroupId,
                sortOrder,
                itemId,
                setItemId,
                itemType,
                displayText
                ));
        }
    }
}
