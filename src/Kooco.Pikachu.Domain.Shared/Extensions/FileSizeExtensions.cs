namespace Kooco.Pikachu.Extensions;

public static class FileSizeExtensions
{
    public static string FromBytesToMB(this int sizeInBytes)
    {
        return (sizeInBytes / (float)(1024 * 1024)).ToString("N2");
    }
}
