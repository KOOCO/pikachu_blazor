using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Assembly;

public class CustomAssemblyContext : AssemblyLoadContext
{
    public virtual nint LoadDinkToPdfDll()
    {
        string dllFolder = string.Empty;

        if (AppContext.BaseDirectory.Contains("home\\site\\wwwroot"))
        {
            dllFolder = string.Concat(AppContext.BaseDirectory.Split("wwwroot\\")[0], "wwwroot\\wwwroot\\libs\\DinkToPdf");
        }

        else
        {
            dllFolder = string.Concat(AppContext.BaseDirectory.Split("pikachu_blazor\\")[0], "pikachu_blazor\\src\\Kooco.Pikachu.Blazor\\wwwroot\\libs\\DinkToPdf");
        }

        string dllPath = Path.Combine(dllFolder, "libwkhtmltox.dll");

        if (File.Exists(dllPath))
        {
            return LoadUnmanagedDllFromPath(dllPath);
        }

        return IntPtr.Zero;
    }
}
