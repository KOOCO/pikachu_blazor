using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities.Auditing;
using Kooco.Pikachu.Items;

namespace Kooco.Pikachu.GroupBuys
{ 
    internal class GroupBuyItemGroup : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 團購編號
        /// </summary>
        public GroupBuy GroupBuy { get; set; }

        /// <summary>
        /// 商品組合排序 Sorting Orger of groupbuy item groupf
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 商品分頁敘述1 TabItemDescription1
        /// </summary>
        public string ItemDescription1 { get; set; }

        /// <summary>
        /// 商品分頁敘述2 TabItemDescription2
        /// </summary>
        public string ItemDescription2 { get; set; }

        /// <summary>
        /// 商品分頁敘述3 TabItemDescription3
        /// </summary>
        public string ItemDescription3 { get; set; }

        /// <summary>
        /// 商品1 Item1
        /// </summary>
        public Item Item1 { get; set; }

        /// <summary>
        /// 商品1排序 Item1Order
        /// </summary>
        public int Item1Order { get; set; }

        /// <summary>
        /// 商品2 Item2
        /// </summary>
        public Item Item2 { get; set; }

        /// <summary>
        /// 商品2排序 Item2Order
        /// </summary>
        public int Item2Order { get; set; }

        /// <summary>
        /// 商品3 Item3
        /// </summary>
        public Item Item3 { get; set; }

        /// <summary>
        /// 商品3排序 Item3Order
        /// </summary>
        public int Item3Order { get; set; }

        /// <summary>
        /// 商品4 Item4
        /// </summary>
        public Item Item4 { get; set; }

        /// <summary>
        /// 商品4排序 Item4Order
        /// </summary>
        public int Item4Order { get; set; }
        
    }
}
