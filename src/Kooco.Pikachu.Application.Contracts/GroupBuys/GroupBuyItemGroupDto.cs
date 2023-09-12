using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupDto :EntityDto<Guid>, IHasConcurrencyStamp
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

        /// <summary>
        /// 商品分頁敘述3 ItemDescription4
        /// </summary>
        public string? ItemDescription4 { get; set; }
        /// <summary>
        /// 商品1 Item1
        /// </summary>
        public Guid? Item1Id { get; set; }

        /// <summary>
        /// 商品1排序 Item1Order
        /// </summary>
        public int? Item1Order { get; set; }

        /// <summary>
        /// 商品2 Item2
        /// </summary>
        public Guid? Item2Id { get; set; }

        /// <summary>
        /// 商品2排序 Item2Order
        /// </summary>
        public int? Item2Order { get; set; }

        /// <summary>
        /// 商品3 Item3
        /// </summary>
        public Guid? Item3Id { get; set; }

        /// <summary>
        /// 商品3排序 Item3Order
        /// </summary>
        public int? Item3Order { get; set; }

        /// <summary>
        /// 商品4 Item4
        /// </summary>
        public Guid? Item4Id { get; set; }


        /// <summary>
        /// 商品4排序 Item4Order
        /// </summary>
        public int? Item4Order { get; set; }

        public string? ConcurrencyStamp {  get; set; }
    }
}
