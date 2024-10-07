using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class OrderMessageDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// The ID of the order associated with the message.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// The optional ID of the sender (either customer or merchant).
        /// </summary>
        public Guid? SenderId { get; set; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The time the message was sent.
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Indicates if the sender is the merchant (true) or customer (false).
        /// </summary>
        public bool IsMerchant { get; set; }
        public string SenderName { get; set; }
    }
}
