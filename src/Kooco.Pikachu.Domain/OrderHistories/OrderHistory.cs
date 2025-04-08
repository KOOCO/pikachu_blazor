using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.OrderHistories
{
    public class OrderHistory : FullAuditedEntity<Guid>
    {
        public Guid OrderId { get; set; }  // The associated order
        public string ActionType { get; set; } // E.g., Created, Updated, RefundRequested, etc.
        public string ActionDetails { get; set; } // JSON or plain text for details

        public Guid? EditorUserId { get; set; } // Nullable for third-party orders
        public string? EditorUserName { get; set; } // Nullable if no user data is available

        public OrderHistory(Guid id,Guid orderId,string actionType,string actionDetails,Guid? editorUserId,string? editorUserName) :base(id)
        {
            OrderId = orderId;
            ActionType = actionType;
            ActionDetails = actionDetails;
            EditorUserId = editorUserId;
            EditorUserName = editorUserName;

        }


        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
