using System.Collections.Generic;

namespace Kooco.Pikachu.ShippingStrategies
{
    /// <summary>
    /// Result of shipping label generation operation
    /// </summary>
    public class ShippingLabelResult
    {
        /// <summary>
        /// Whether the label generation was successful
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Generated label content (PDF, image, etc.)
        /// </summary>
        public byte[] LabelContent { get; set; } = new byte[0];
        
        /// <summary>
        /// Content type of the generated label
        /// </summary>
        public string ContentType { get; set; } = string.Empty;
        
        /// <summary>
        /// Label file name
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// URL to download the label (if applicable)
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Error messages if generation failed
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();
        
        /// <summary>
        /// Additional metadata from label generation
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}