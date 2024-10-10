using System;

namespace Kooco.Pikachu.Extensions;

public static class FileSizeExtensions
{
    public static string FromBytesToMB(this int sizeInBytes)
    {
        return (sizeInBytes / (float)(1024 * 1024)).ToString("N2");
    }

    public static string ExtractFileName(this string? url)
    {
        if (url.IsNullOrWhiteSpace())
        {
            return "";
        }

        return url.Substring((url.LastIndexOf('/') + 1));
    }
}
