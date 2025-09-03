using Blazorise;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Helpers;

public static class FileHelper
{
    public static async Task<byte[]> GetBytes(this IFileEntry file)
    {
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(long.MaxValue).CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public static async Task<byte[]> GetBytes(this IBrowserFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(long.MaxValue).CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
