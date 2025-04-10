namespace Kooco;
public sealed class ECPayOptions
{
    public required bool IsFormalArea { get; set; }
    public required string Id { get; set; }
    public required string HashKey { get; set; }
    public required string HashIV { get; set; }
}