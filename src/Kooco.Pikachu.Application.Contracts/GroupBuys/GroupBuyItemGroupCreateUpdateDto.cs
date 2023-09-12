using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupCreateUpdateDto
    {
        public Guid? Id { get; set; }

        public Guid? TenantId { get; set; }

        /// <summary>
        /// 團購編號
        /// </summary>
        public Guid GroupBuyId { get; set; }

        /// <summary>
        /// 商品組合排序 Sorting Orger of groupbuy item groupf
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 商品分頁敘述1 Title
        /// </summary>
        
        public string? Title { get; set; }


        public ICollection<GroupBuyItemGroupDetailCreateUpdateDto> ItemDetails { get; set; }

        public GroupBuyItemGroupCreateUpdateDto()
        {
            ItemDetails = new List<GroupBuyItemGroupDetailCreateUpdateDto>();
        }
    }
}
