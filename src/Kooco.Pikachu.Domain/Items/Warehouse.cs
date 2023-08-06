using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Items
{
    public class Warehouse : FullAuditedAggregateRoot<Guid> , IMultiTenant
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 倉庫名稱 Warehouse Name
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 倉庫地址 Warehouse Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 聯絡人 Contact Person
        /// </summary>
        public string ContactPerson { get; set; }
        /// <summary>
        /// 聯絡電話 Contact Number
        /// </summary>
        public string ContactNumber { get; set; }
        /// <summary>
        /// 倉庫狀態 Warehouse Status
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 是否為預設倉庫 Is Default Warehouse
        /// </summary>
        public bool isDefault { get; set; }
    }
}
