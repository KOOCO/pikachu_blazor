using Microsoft.JSInterop;
using MiniExcelLibs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Blazor.Helpers;
public class ExcelDownloadHelper(IJSRuntime JSRuntime)
{
    public async Task DownloadExcelAsync(IRemoteStreamContent remoteStreamContent)
    {
        using var responseStream = remoteStreamContent.GetStream();
        // Create Excel file from the stream
        using var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Convert MemoryStream to byte array
        var excelData = memoryStream.ToArray();

        // Trigger the download using JavaScript interop
        await JSRuntime.InvokeVoidAsync("downloadFile", new
        {
            ByteArray = excelData,
            remoteStreamContent.FileName,
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }

    public async Task DownloadCsvAsync(IRemoteStreamContent remoteStreamContent)
    {
        using var responseStream = remoteStreamContent.GetStream();
        using var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var csvData = memoryStream.ToArray();

        await JSRuntime.InvokeVoidAsync("downloadFile", new
        {
            ByteArray = csvData,
            remoteStreamContent.FileName, // should already end with .csv
            ContentType = "text/csv"
        });
    }

    public async Task DownloadExcelAsync(byte[] excelData, string fileName)
    {
        await JSRuntime.InvokeVoidAsync("downloadFile", new
        {
            ByteArray = excelData,
            FileName = fileName,
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }

    public async Task DownloadCsvAsync(byte[] csvData, string fileName)
    {
        await JSRuntime.InvokeVoidAsync("downloadFile", new
        {
            ByteArray = csvData,
            FileName = fileName,
            ContentType = "text/csv"
        });
    }

    public async Task DownloadExcelAsync<T>(List<T> dataList, string fileName)
    {
        using var memoryStream = new MemoryStream();

        memoryStream.SaveAs(dataList);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var excelData = memoryStream.ToArray();

        await JSRuntime.InvokeVoidAsync("downloadFile", new
        {
            ByteArray = excelData,
            FileName = fileName,
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }
}