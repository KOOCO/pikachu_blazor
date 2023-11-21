using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.OrderDeliveries
{
    public class OrderDelivery : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public  DeliveryMethod DeliveryMethod { get; set; }
        public  DeliveryStatus DeliveryStatus { get; set; }
        public string? DeliveryNo { get; set; }
        public int? CarrierId { get; set; }
        public Guid OrderId { get; set; }
        public ICollection<OrderItem> Items  { get; set; }

        public Guid? TenantId { get; set; }
        [ForeignKey(nameof(CarrierId))]
        public EnumValue Carrier { get; set; }

        public OrderDelivery(Guid id, DeliveryMethod deliveryMethod,DeliveryStatus deliveryStatus,string? deliveryNo,int? carrierId,Guid orderId )

        {
            Id = id;
        DeliveryMethod = deliveryMethod;
            DeliveryStatus = deliveryStatus;
            CarrierId = carrierId;
            DeliveryNo= deliveryNo;
            OrderId = orderId;
        
        }
      
    }
}
