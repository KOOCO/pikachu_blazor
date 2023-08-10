using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class ItemInventory : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 產品編號 Item Id
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// 倉庫編號 Warehouse Id
        /// </summary>
        public Warehouse Warehouse { get; set; }
        /// <summary>
        /// 庫存量 QuantityOnHand - This is the quantity of the item that is currently in stock in this warehouse.
        /// </summary>
        public int QuantityOnHand { get; set; }

        /// <summary>
        /// 安全庫存量 Safty Quantity
        /// </summary>
        public int SaftyQuantity { get; set; }

        /// <summary>
        /// 是否需要通知 Need Notify?
        /// </summary>
        public bool needNotify { get; set; } = false;
    }
}
