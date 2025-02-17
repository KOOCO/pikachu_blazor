using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class OrderHistoryDto: FullAuditedEntityDto<Guid>
    {
        public Guid OrderId { get; set; }  // The associated order
        public string ActionType { get; set; } // E.g., Created, Updated, RefundRequested, etc.
        public string ActionDetails { get; set; } // JSON or plain text for details

        public Guid? EditorUserId { get; set; } // Nullable for third-party orders
        public string? EditorUserName { get; set; } // Nullable if no user data is available
    }
}
