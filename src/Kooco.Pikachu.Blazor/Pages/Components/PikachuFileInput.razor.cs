using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Components;

public partial class PikachuFileInput
{
    [Parameter] public EventCallback<InputFileChangeEventArgs> OnChanged { get; set; }
    [Parameter] public EventCallback<PikachuFileInfo> FileInfoChanged { get; set; }
    [Parameter] public EventCallback<List<PikachuFileInfo>> FileInfosChanged { get; set; }

    [Parameter] public string Accept { get; set; } = string.Empty;
    [Parameter] public PikachuFileType? FileType { get; set; }

    [Parameter] public double SizeMb { get; set; } = 10;
    [Parameter] public bool Multiple { get; set; }
    [Parameter] public int MaxFiles { get; set; } = 10;
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Processing { get; set; }

    private List<PikachuFileInfo> FileInfos { get; set; } = [];
    private List<string> ErrorMessages { get; set; } = [];

    private readonly string _randomId = Guid.NewGuid().ToString("N");
    private string InputId => $"pikachuFileInput_{_randomId}";

    private string AcceptString => !string.IsNullOrWhiteSpace(Accept) ? Accept : GetAcceptFromFileType();

    private string SupportedFiles => string.IsNullOrWhiteSpace(AcceptString)
        ? L["AllFiles"]
        : string.Join(", ", AcceptString.Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim().TrimStart('.').ToUpper()));

    private bool IsDisabled => Disabled || Processing;
    private string GetDisabledClass() => IsDisabled ? "disabled" : string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initPikachuFileInput", _randomId, Multiple);
        }
    }

    private string GetAcceptFromFileType()
    {
        return FileType switch
        {
            PikachuFileType.Excel => ".xlsx,.xls",
            PikachuFileType.Csv => ".csv",
            PikachuFileType.ExcelCsv => ".xlsx,.xls,.csv",
            PikachuFileType.Image => ".jpg,.jpeg,.png,.gif,.bmp,.webp,.svg",
            PikachuFileType.Document => ".pdf, .doc, .docx",
            PikachuFileType.Text => ".txt",
            _ => string.Empty
        };
    }

    public async Task OnFileChange(InputFileChangeEventArgs args)
    {
        try
        {
            if (IsDisabled) return;
            
            ErrorMessages.Clear();

            await OnChanged.InvokeAsync(args);

            var filesToProcess = Multiple ? args.GetMultipleFiles(MaxFiles) : [args.File];
            var validFiles = new List<PikachuFileInfo>();

            foreach (var file in filesToProcess)
            {
                if (file == null) continue;

                // Check file size
                if (file.Size > SizeMb * 1024 * 1024)
                {
                    ErrorMessages.Add(L["PikachuFileInput:InvalidFileSize", file.Name]);
                    continue;
                }

                // Check file extension
                string ext = Path.GetExtension(file.Name);
                if (!string.IsNullOrWhiteSpace(AcceptString) && !AcceptString.Split(",").Any(a => a.Trim().Equals(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    ErrorMessages.Add(L["PikachuFileInput:InvalidFileExtension", file.Name]);
                    continue;
                }

                // Process valid file
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream(long.MaxValue).CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                var fileInfo = new PikachuFileInfo
                {
                    Name = file.Name,
                    Extension = ext,
                    File = file,
                    FileBytes = bytes,
                    FileStream = new MemoryStream(bytes)
                };

                validFiles.Add(fileInfo);
            }

            if (validFiles.Count != 0)
            {
                if (Multiple)
                {
                    FileInfos.AddRange(validFiles);
                    await FileInfosChanged.InvokeAsync(FileInfos);
                }
                else
                {
                    FileInfos.Clear();
                    var file = validFiles.First();
                    FileInfos.Add(file);
                    await FileInfoChanged.InvokeAsync(file);
                }

                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public async Task RemoveFile(PikachuFileInfo fileInfo)
    {
        try
        {
            if (IsDisabled) return;

            FileInfos.Remove(fileInfo);
            
            fileInfo.FileStream?.Dispose();

            if (Multiple)
            {
                await FileInfosChanged.InvokeAsync(FileInfos);
            }
            else
            {
                await FileInfoChanged.InvokeAsync(FileInfos.FirstOrDefault());
            }

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public void ClearFiles()
    {
        if (IsDisabled) return;

        foreach (var fileInfo in FileInfos)
        {
            fileInfo.FileStream?.Dispose();
        }
        FileInfos.Clear();

        if (Multiple)
            _ = FileInfosChanged.InvokeAsync(FileInfos);
        else
            _ = FileInfoChanged.InvokeAsync(null);

        InvokeAsync(StateHasChanged);
    }


    static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearFiles();

            // cleanup JS bindings
            _ = JSRuntime.InvokeVoidAsync("cleanupPikachuFileInput", _randomId);
        }
        base.Dispose(disposing);
    }

    // Public methods for external access
    public List<PikachuFileInfo> GetFiles() => [.. FileInfos];
    public PikachuFileInfo? GetFile() => FileInfos.FirstOrDefault();
    public bool HasFiles() => FileInfos is { Count: > 0 };
}

public class PikachuFileInfo : IDisposable
{
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public IBrowserFile? File { get; set; }
    public byte[] FileBytes { get; set; } = [];
    public Stream? FileStream { get; set; }

    public void Dispose()
    {
        FileStream?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public enum PikachuFileType
{
    Excel,
    Csv,
    ExcelCsv,
    Image,
    Document,
    Text
}