using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountCodeUsage:AuditedAggregateRoot<Guid>
    {
        public Guid DiscountCodeId { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalDiscountAmount { get; set; }
        [ForeignKey(nameof(DiscountCodeId))]
        public DiscountCode? DiscountCode { get; set; }

       public DiscountCodeUsage(Guid id,int totalOrders,int totalUsers,int totalDiscountAmount):base(id) { 
        TotalOrders= totalOrders;
            TotalUsers= totalUsers;
            TotalDiscountAmount = totalDiscountAmount;
        
        }
    }
}
