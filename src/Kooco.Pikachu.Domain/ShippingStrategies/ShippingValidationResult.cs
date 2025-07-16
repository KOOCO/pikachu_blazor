using System.Collections.Generic;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Result of shipping data validation operation
    /// </summary>
    public class ShippingValidationResult
    {
        /// <summary>
        /// Whether the validation was successful
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// Validation error messages
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Warning messages (non-blocking)
        /// </summary>
        public List<string> WarningMessages { get; set; } = new();
        
        /// <summary>
        /// Field-specific validation results
        /// </summary>
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new();
        
        /// <summary>
        /// Additional validation metadata
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}