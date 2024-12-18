using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.OrderDeliveries
{
    public class OrderDeliveryDto : FullAuditedAggregateRoot<Guid>
    {
        public DeliveryMethod DeliveryMethod { get; set; }
        public DeliveryMethod? ActualDeliveryMethod { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string AllPayLogisticsID { get; set; }
		public string? StatusName { get; set; }
		public string? StatusId { get; set; }
		public string Editor { get; set; }
        public string? DeliveryNo { get; set; }
        public string? FileNo { get; set; }
        public Guid OrderId { get; set; }
        public List<OrderItemDto> Items { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}
