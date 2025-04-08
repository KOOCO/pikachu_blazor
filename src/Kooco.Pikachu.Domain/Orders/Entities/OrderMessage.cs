using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders.Entities
{
    public class OrderMessage:AuditedEntity<Guid>
    {
        public Guid OrderId { get; set; } // Required: ID of the order associated with the message

        public Guid? SenderId { get; set; } // Optional: ID of the sender (either customer or merchant)

        public string Message { get; set; } // Required: Content of the message

        public DateTime? Timestamp { get; set; } // Optional: Time the message was sent (server will default to current time if not provided)

        public bool IsMerchant { get; set; } // Required: Indicates if the sender is the merchant (true) or customer (false)
        [NotMapped]
        public string SenderName { get; set; }
        public OrderMessage() { }

        public OrderMessage(Guid id,Guid orderId, Guid? senderId, string message,bool isMerchant):base(id)
            
        {
            SenderId = senderId;
            OrderId = orderId;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            IsMerchant = isMerchant;
            Timestamp = DateTime.UtcNow; // Default to current UTC time if no timestamp provided
        }
    }
}
