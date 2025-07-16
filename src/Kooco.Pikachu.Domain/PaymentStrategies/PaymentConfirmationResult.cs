using System.Collections.Generic;

namespace Kooco.Pikachu.PaymentStrategies
{
    /// <summary>
    /// Result of payment confirmation operation
    /// </summary>
    public class PaymentConfirmationResult
    {
        /// <summary>
        /// Whether the payment confirmation was successful
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Transaction ID from payment gateway
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;
        
        /// <summary>
        /// Payment gateway confirmation response data
        /// </summary>
        public object ResponseData { get; set; } = new();
        
        /// <summary>
        /// Error messages if confirmation failed
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Additional metadata from payment gateway
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}