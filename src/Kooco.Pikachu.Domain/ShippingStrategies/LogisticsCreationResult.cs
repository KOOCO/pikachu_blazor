using System.Collections.Generic;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Result of logistics order creation operation
    /// </summary>
    public class LogisticsCreationResult
    {
        /// <summary>
        /// Whether the logistics creation was successful
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Generated logistics order ID
        /// </summary>
        public string LogisticsOrderId { get; set; } = string.Empty;
        
        /// <summary>
        /// Tracking number for the shipment
        /// </summary>
        public string TrackingNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Logistics provider response data
        /// </summary>
        public object ResponseData { get; set; } = new();
        
        /// <summary>
        /// Error messages if creation failed
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Additional metadata from logistics provider
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}