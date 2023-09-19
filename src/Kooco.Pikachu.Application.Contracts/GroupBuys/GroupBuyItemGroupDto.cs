using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupDto : EntityDto<Guid>, IHasConcurrencyStamp
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

        public ICollection<GroupBuyItemGroupDetailsDto> ItemGroupDetails { get; set; }

        public string? ConcurrencyStamp { get; set; }
    }
}
