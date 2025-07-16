using System.Collections.Generic;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Result of shipping cost calculation operation
    /// </summary>
    public class ShippingCostResult
    {
        /// <summary>
        /// Whether the cost calculation was successful
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Calculated shipping cost
        /// </summary>
        public decimal Cost { get; set; }
        
        /// <summary>
        /// Breakdown of cost components
        /// </summary>
        public Dictionary<string, decimal> CostBreakdown { get; set; } = new();
        
        /// <summary>
        /// Error messages if calculation failed
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Additional metadata about the calculation
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}