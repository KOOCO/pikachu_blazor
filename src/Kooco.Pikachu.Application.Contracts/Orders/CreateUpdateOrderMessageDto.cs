using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Orders
{
    public class CreateUpdateOrderMessageDto
    {
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

    }
}
