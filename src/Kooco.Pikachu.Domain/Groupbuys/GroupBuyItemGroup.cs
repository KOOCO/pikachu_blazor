using System;
using Volo.Abp.MultiTenancy;
using Kooco.Pikachu.Items;
using Volo.Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.CodeDom.Compiler;

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

        /// <summary>
        /// 商品分頁敘述1 TabItemDescription1
        /// </summary>
        public string? ItemDescription1 { get; set; }

        /// <summary>
        /// 商品分頁敘述2 TabItemDescription2
        /// </summary>
        public string? ItemDescription2 { get; set; }

        /// <summary>
        /// 商品分頁敘述3 TabItemDescription3
        /// </summary>
        public string? ItemDescription3 { get; set; }
        public string? ItemDescription4 { get; set; }
        /// <summary>
        /// 商品1 Item1
        /// </summary>
        [ForeignKey(nameof(Item1))]
        public Guid? Item1Id { get; set; }
        public Item? Item1 { get; set; }

        /// <summary>
        /// 商品1排序 Item1Order
        /// </summary>
        public int? Item1Order { get; set; }

        /// <summary>
        /// 商品2 Item2
        /// </summary>
        [ForeignKey(nameof(Item2))]
        public Guid? Item2Id { get; set; }
        public Item? Item2 { get; set; }

        /// <summary>
        /// 商品2排序 Item2Order
        /// </summary>
        public int? Item2Order { get; set; }

        /// <summary>
        /// 商品3 Item3
        /// </summary>
        [ForeignKey(nameof(Item3))]
        public Guid? Item3Id { get; set; }
        public Item? Item3 { get; set; }

        /// <summary>
        /// 商品3排序 Item3Order
        /// </summary>
        public int? Item3Order { get; set; }

        /// <summary>
        /// 商品4 Item4
        /// </summary>
        [ForeignKey(nameof(Item4))]
        public Guid? Item4Id { get; set; }
        public Item? Item4 { get; set; }
       

        /// <summary>
        /// 商品4排序 Item4Order
        /// </summary>
        public int? Item4Order { get; set; }

        public GroupBuyItemGroup()
        {
            
        }

        public GroupBuyItemGroup(
            Guid id,
            Guid groupBuyId,
            int sortOrder,
            Guid? item1Id,
            int? item1Order,
            string? itemDescription1,
            Guid? item2Id,
            int? item2Order,
            string? itemDescription2,
            Guid? item3Id,
            int? item3Order,
            string? itemDescription3,
              Guid? item4Id,
            int? item4Order,
            string? itemDescription4
            ) : base(id)
        {
            GroupBuyId = groupBuyId;
            SortOrder = sortOrder;
            Item1Id = item1Id;
            Item1Order = item1Order;
            ItemDescription1 = itemDescription1;
            Item2Id = item2Id;
            Item2Order = item2Order;
            ItemDescription2 = itemDescription2;
            Item3Id = item3Id;
            Item3Order = item3Order;
            ItemDescription3 = itemDescription3;
            Item4Id = item4Id;
           Item4Order = item4Order;
            ItemDescription4 = itemDescription4;
        }
    }
}
