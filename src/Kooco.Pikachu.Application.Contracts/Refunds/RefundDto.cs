using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Refunds
{
    public class RefundDto : FullAuditedEntityDto<Guid>
    {
        public Guid OrderId { get; set; }
        public DateTime? ReviewCompletionTime { get; set; }
        public RefundReviewStatus RefundReview { get; set; }
        public OrderDto Order { get; set; }
        public Guid? TenantId { get; set; }
        public string? Approver { get; set; }
        public string? Refunder { get; set; }
    }
}
