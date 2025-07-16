using System.Collections.Generic;

namespace Kooco.Pikachu.PaymentStrategies
{
    /// <summary>
    /// Result of refund processing operation
    /// </summary>
    public class RefundProcessingResult
    {
        /// <summary>
        /// Whether the refund processing was successful
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Refund transaction ID from payment gateway
        /// </summary>
        public string RefundTransactionId { get; set; } = string.Empty;
        
        /// <summary>
        /// Original transaction ID that was refunded
        /// </summary>
        public string OriginalTransactionId { get; set; } = string.Empty;
        
        /// <summary>
        /// Amount that was refunded
        /// </summary>
        public decimal RefundedAmount { get; set; }
        
        /// <summary>
        /// Payment gateway refund response data
        /// </summary>
        public object ResponseData { get; set; } = new();
        
        /// <summary>
        /// Error messages if refund failed
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Additional metadata from payment gateway
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}