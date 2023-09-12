using System;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities;
using Kooco.Pikachu.Groupbuys;
using System.Collections.Generic;

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

        public string? Title { get; set; }

        public ICollection<GroupBuyItemGroupDetails> ItemDetails { get; set; }

        public GroupBuyItemGroup()
        {
            
        }

        public GroupBuyItemGroup(
            Guid id,
            Guid groupBuyId,
            int sortOrder,
            string? title
            ) : base(id)
        {
            GroupBuyId = groupBuyId;
            SortOrder = sortOrder;
            Title = title;
            ItemDetails = new List<GroupBuyItemGroupDetails>();
        }

        public void GroupBuyItemGroupDetails(
            Guid id,
            Guid groupBuyItemGroupId,
            int sortOrder,
            string? itemDescription,
            Guid? itemId,
            Guid? imageId
            
            )
        {
            ItemDetails.Add(new GroupBuyItemGroupDetails(
                id,
                groupBuyItemGroupId,
                sortOrder,
                itemDescription,
                itemId,
                imageId
                ));
        }
    }
}
