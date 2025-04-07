namespace Kooco.Pikachu;
public sealed class ECPayOptions
{
    public required bool IsFormalArea { get; set; }
    public required MediaInfo Merchant { get; set; }
    public required MediaInfo Platform { get; set; }
    public sealed class MediaInfo
    {
        public required string Id { get; set; }
        public required string HashKey { get; set; }
        public required string HashIV { get; set; }
        public sealed class EinvoiceInfo
        {
            public required string HashKey { get; set; }
            public required string HashIV { get; set; }
        }
    }
}