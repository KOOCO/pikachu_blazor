using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Refunds
{
    public class Refund : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid OrderId { get; set; }

        public DateTime? ReviewCompletionTime { get; set; }
        public RefundReviewStatus RefundReview { get; set; }
        public string? Approver { get; set; }
        public string? Refunder { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
        public Guid? TenantId { get; set; }

        public Refund()
        {
            
        }

        public Refund(
            Guid id,
            Guid orderId
            ) : base(id)
        {
            OrderId = orderId;
        }
    }
}
