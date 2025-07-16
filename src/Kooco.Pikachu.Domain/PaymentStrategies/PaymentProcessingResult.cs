using System.Collections.Generic;

namespace Kooco.Pikachu.PaymentStrategies
{
    /// <summary>
    /// Result of payment processing operation
    /// </summary>
    public class PaymentProcessingResult
    {
        /// <summary>
        /// Whether the payment processing was successful
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Transaction ID from payment gateway
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;
        
        /// <summary>
        /// Payment gateway response data
        /// </summary>
        public object ResponseData { get; set; } = new();
        
        /// <summary>
        /// HTML form to redirect to payment gateway (for form-based payments)
        /// </summary>
        public string? RedirectForm { get; set; }
        
        /// <summary>
        /// Direct URL to redirect to payment gateway (for URL-based payments)
        /// </summary>
        public string? RedirectUrl { get; set; }
        
        /// <summary>
        /// Error messages if processing failed
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Additional metadata from payment gateway
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}