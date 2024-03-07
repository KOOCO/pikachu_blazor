using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.OrderDeliveries
{
    public class OrderDeliveryDto : FullAuditedAggregateRoot<Guid>
    {
        public DeliveryMethod DeliveryMethod { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string AllPayLogisticsID { get; set; }
        public string Editor { get; set; }
        public string? DeliveryNo { get; set; }
      
        public Guid OrderId { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
