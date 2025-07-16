using System.Collections.Generic;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Result of shipping status update operation
    /// </summary>
    public class ShippingStatusUpdateResult
    {
        /// <summary>
        /// Whether the status update was successful
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Previous status before update
        /// </summary>
        public string PreviousStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// New status after update
        /// </summary>
        public string NewStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// Timestamp of the status change
        /// </summary>
        public System.DateTime UpdatedAt { get; set; } = System.DateTime.Now;
        
        /// <summary>
        /// Error messages if update failed
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Additional metadata from status update
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}