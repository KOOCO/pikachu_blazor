using System.Text.Json.Serialization;

namespace Kooco.Pikachu.ImageCompressors;

public class CompressImageResponse
{
    [JsonPropertyName("originalSize")]
    public long OriginalSize { get; set; }

    [JsonPropertyName("compressedSize")]
    public long CompressedSize { get; set; }

    [JsonPropertyName("compressionRatio")]
    public double CompressionRatio { get; set; }

    [JsonPropertyName("compressedImage")]
    public string CompressedImage { get; set; }

    public byte[] CompressedBytes { get; set; }
}
