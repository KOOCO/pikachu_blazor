using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities;
using Kooco.Pikachu.Items;

namespace Kooco.Pikachu.CommonServices.ItemImages
{
    internal class ItemImage : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 商品編號 Item Id
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// 圖片網址 Image URL
        /// </summary>
        public string ImageURL { get; set; }

        /// <summary>
        /// 圖片排序 Image Sort Order
        /// </summary>
        public int SortOrder { get; set; }
        
    }
}
