using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Blazorise.Utilities;
using DinkToPdf;
using HtmlAgilityPack;
using Hangfire.Server;
using Kooco.Pikachu.Assembly;
using Kooco.Pikachu.Blazor.Pages.ItemManagement;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.StoreLogisticOrders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUglify.Html;
using OneOf.Types;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Account.Web;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using static Kooco.Pikachu.Permissions.PikachuPermissions;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using MudBlazor;
using static Volo.Abp.Ui.LayoutHooks.LayoutHooks;

namespace Kooco.Pikachu.Blazor.Pages.Orders;

public partial class Order
{
    #region Inject
    private Dictionary<Guid, List<OrderDeliveryDto>> OrderDeliveriesByOrderId { get; set; } = [];
    private static readonly SynchronizedConverter Converter = new SynchronizedConverter(new PdfTools());
    private bool IsAllSelected { get; set; } = false;
    private List<OrderDeliveryDto> OrderDeliveries { get; set; }
    private List<OrderDto> Orders { get; set; } = new();
    private int TotalCount { get; set; }
    private OrderDto SelectedOrder { get; set; }
    private OrderItemDto SelectedOrderItem { get; set; }
    private OrderDeliveryDto SelectedOrderDelivery { get; set; }
    private Guid OrderDeliveryId { get; set; }
    private Guid? GroupBuyFilter { get; set; }
    private int PageIndex { get; set; } = 1;
    private int PageSize { get; set; } = 10;
    private Guid? SelectedGroupBuy { get; set; }
    private DateTime? StartDate { get; set; }
    private DateTime? EndDate { get; set; }
    private string? Sorting { get; set; }
    private string? Filter { get; set; }
    private bool isOrderCombine { get; set; } = false;
    private readonly HashSet<Guid> ExpandedRows = new();
    private LoadingIndicator loading { get; set; }
    private List<KeyValueDto> GroupBuyList { get; set; } = new();
    private List<ShippingStatus> ShippingStatuses { get; set; } = [];
    private List<DeliveryMethod> DeliveryMethods { get; set; } = [];
    private List<OrderDto> OrdersSelected = [];
    private string SelectedTabName = "All";

    private int NormalCount = 0;
    private int FreezeCount = 0;
    private int FrozenCount = 0;
    private DeliveryMethod? DeliveryMethod = null;

    private Guid? ExpandedOrderId = null;

    private bool IsDeliveryNoExists = false;
    private bool FiltersVisible { get; set; } = false;
    #endregion

    #region Methods
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
    {
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "mySelectElement");
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "shippingMethodSelectElem");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "startDate");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "endDate");
        PageIndex = e.Page - 1;
        await UpdateItemList();
        await GetGroupBuyList();
        await InvokeAsync(StateHasChanged);
    }

    public void SelectedTabChanged(string e)
    {
        SelectedTabName = e;

        TotalCount = 0;

        StateHasChanged();
    }

    public async Task OnSeparatePrintShippedLabel(MouseEventArgs e)
    {
        await loading.Show();

        List<string> allPayLogisticsId = [];

        List<Guid> orderIds = [.. Orders.Where(w => w.IsSelected).Select(s => s.OrderId)];

        foreach (Guid orderId in orderIds)
        {
            OrderDto order = await _orderAppService.GetAsync(orderId);

            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(orderId);

            allPayLogisticsId.AddRange([.. orderDeliveries.Where(w => !w.AllPayLogisticsID.IsNullOrWhiteSpace()).Select(s => s.AllPayLogisticsID)]);
        }

        string html = await _StoreLogisticsOrderAppService.OnBatchPrintingShippingLabel(allPayLogisticsId);

        await JSRuntime.InvokeVoidAsync("PrintTradeDocument", html);

        await loading.Hide();
    }

    public async Task OnPrintShippedLabel(MouseEventArgs e)
    {
        await loading.Show();

        List<Guid> orderIds = Orders.Where(w => w.IsSelected).Select(s => s.OrderId).ToList();

        Dictionary<string, string> AllPayLogisticsIds = new() 
        { 
            { "BlackCat1", string.Empty },
            { "BlackCatFrozen", string.Empty },
            { "BlackCatFreeze", string.Empty },
            { "SevenToElevenFrozen", string.Empty },
            { "PostOffice", string.Empty },
            { "FamilyMart1", string.Empty },
            { "SevenToEleven1", string.Empty },
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty },
        };

        Dictionary<string, string> allPayLogistic = new()
        {
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty }
        };

        Dictionary<string, string> DeliveryNumbers = new()
        {
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
        };

        foreach (Guid orderId in orderIds)
        {
            OrderDto order = await _orderAppService.GetAsync(orderId);

            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(orderId);

            foreach (OrderDeliveryDto? delivery in orderDeliveries)
            {
                if (!string.IsNullOrWhiteSpace(delivery.AllPayLogisticsID) && 
                    (!(delivery.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.DeliveredByStore)))
                {
                    string? AllPayLogisticsIdsValue = AllPayLogisticsIds.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                    List<string> AllPayLogisticId = AllPayLogisticsIdsValue.IsNullOrEmpty() ? [] : [.. AllPayLogisticsIdsValue.Split(',')];

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                    {
                        AllPayLogisticId.Add(delivery.FileNo);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));

                        if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                        {
                            string? aplValues = allPayLogistic.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                            List<string> aPL = aplValues.IsNullOrEmpty() ? [] : [.. aplValues.Split(',')];

                            aPL.Add(delivery.AllPayLogisticsID);

                            allPayLogistic.Remove(delivery.DeliveryMethod.ToString());

                            allPayLogistic.Add(delivery.DeliveryMethod.ToString(), string.Join(",", aPL));
                        }
                    }

                    else
                    {
                        AllPayLogisticId.Add(delivery.AllPayLogisticsID);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));
                    }

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C || 
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                    {
                        string? DeliveryNumberValue = DeliveryNumbers.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                        List<string> DeliveryNumber = DeliveryNumberValue.IsNullOrEmpty() ? [] : [.. DeliveryNumberValue.Split(',')];

                        DeliveryNumber.Add(delivery.DeliveryNo);

                        DeliveryNumbers.Remove(delivery.DeliveryMethod.ToString());

                        DeliveryNumbers.Add(delivery.DeliveryMethod.ToString(), string.Join(",", DeliveryNumber));
                    }
                }
            }
        }

        string MergeTempFolder = Path.Combine(Path.GetTempPath(), "MergeTemp");

        Directory.CreateDirectory(MergeTempFolder);

        Tuple<List<string>, List<string>, List<string>> tuple = await _StoreLogisticsOrderAppService.OnBatchPrintingShippingLabel(AllPayLogisticsIds, DeliveryNumbers, allPayLogistic);

        string errors = string.Join('\n', tuple.Item3);

        if (!errors.IsNullOrWhiteSpace()) await _uiMessageService.Warn(errors);

        List<string> outputPdfPaths = GeneratePdf(tuple.Item1);

        if (tuple.Item2 is { Count: > 0 }) outputPdfPaths.AddRange(tuple.Item2);

        if (outputPdfPaths is { Count: > 0 })
        {
            MemoryStream combinedPdfStream = CombinePdf(outputPdfPaths);

            await JSRuntime.InvokeVoidAsync("downloadFile", new
            {
                ByteArray = combinedPdfStream.ToArray(),
                FileName = "Invoices.pdf",
                ContentType = "application/pdf"
            });
        }

        Directory.Delete(Path.Combine(Path.GetTempPath(), "MergeTemp"), true);

        await loading.Hide();
    }

    public async Task OnPrintShippedLabel10Cm()
    {
        await loading.Show();

        List<Guid> orderIds = Orders.Where(w => w.IsSelected).Select(s => s.OrderId).ToList();

        Dictionary<string, string> AllPayLogisticsIds = new()
        {
            { "BlackCat1", string.Empty },
            { "BlackCatFrozen", string.Empty },
            { "BlackCatFreeze", string.Empty },
            { "SevenToElevenFrozen", string.Empty },
            { "PostOffice", string.Empty },
            { "FamilyMart1", string.Empty },
            { "SevenToEleven1", string.Empty },
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty },
        };

        Dictionary<string, string> allPayLogistic = new()
        {
            { "TCatDeliverySevenElevenNormal", string.Empty },
            { "TCatDeliverySevenElevenFreeze", string.Empty },
            { "TCatDeliverySevenElevenFrozen", string.Empty }
        };

        Dictionary<string, string> DeliveryNumbers = new()
        {
            { "SevenToElevenC2C", string.Empty },
            { "FamilyMartC2C", string.Empty },
            { "TCatDeliveryNormal", string.Empty },
            { "TCatDeliveryFreeze", string.Empty },
            { "TCatDeliveryFrozen", string.Empty },
        };

        foreach (Guid orderId in orderIds)
        {
            OrderDto order = await _orderAppService.GetAsync(orderId);

            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(orderId);

            foreach (OrderDeliveryDto? delivery in orderDeliveries)
            {
                if (!string.IsNullOrWhiteSpace(delivery.AllPayLogisticsID) &&
                    (!(delivery.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery ||
                     delivery.DeliveryMethod is EnumValues.DeliveryMethod.DeliveredByStore)))
                {
                    string? AllPayLogisticsIdsValue = AllPayLogisticsIds.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                    List<string> AllPayLogisticId = AllPayLogisticsIdsValue.IsNullOrEmpty() ? [] : [.. AllPayLogisticsIdsValue.Split(',')];

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                    {
                        AllPayLogisticId.Add(delivery.FileNo);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));

                        if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                            delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                        {
                            string? aplValues = allPayLogistic.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                            List<string> aPL = aplValues.IsNullOrEmpty() ? [] : [.. aplValues.Split(',')];

                            aPL.Add(delivery.AllPayLogisticsID);

                            allPayLogistic.Remove(delivery.DeliveryMethod.ToString());

                            allPayLogistic.Add(delivery.DeliveryMethod.ToString(), string.Join(",", aPL));
                        }
                    }

                    else
                    {
                        AllPayLogisticId.Add(delivery.AllPayLogisticsID);

                        AllPayLogisticsIds.Remove(delivery.DeliveryMethod.ToString());

                        AllPayLogisticsIds.Add(delivery.DeliveryMethod.ToString(), string.Join(",", AllPayLogisticId));
                    }

                    if (delivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                        delivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                    {
                        string? DeliveryNumberValue = DeliveryNumbers.GetValueOrDefault(delivery.DeliveryMethod.ToString()) ?? string.Empty;

                        List<string> DeliveryNumber = DeliveryNumberValue.IsNullOrEmpty() ? [] : [.. DeliveryNumberValue.Split(',')];

                        DeliveryNumber.Add(delivery.DeliveryNo);

                        DeliveryNumbers.Remove(delivery.DeliveryMethod.ToString());

                        DeliveryNumbers.Add(delivery.DeliveryMethod.ToString(), string.Join(",", DeliveryNumber));
                    }
                }
            }
        }

        string MergeTempFolder = Path.Combine(Path.GetTempPath(), "MergeTemp");

        Directory.CreateDirectory(MergeTempFolder);

        Tuple<List<string>, List<string>, List<string>> tuple = await _StoreLogisticsOrderAppService.OnBatchPrintingShippingLabel(AllPayLogisticsIds, DeliveryNumbers, allPayLogistic);

        string errors = string.Join('\n', tuple.Item3);

        if (!errors.IsNullOrWhiteSpace()) await _uiMessageService.Warn(errors);

        List<string> outputPdfPaths = GenerateA6Pdf(tuple.Item1);

        if (tuple.Item2 is { Count: > 0 }) outputPdfPaths.AddRange(tuple.Item2);

        if (outputPdfPaths is { Count: > 0 })
        {
            MemoryStream combinedPdfStream = CombinePdf(outputPdfPaths);

            await JSRuntime.InvokeVoidAsync("downloadFile", new
            {
                ByteArray = combinedPdfStream.ToArray(),
                FileName = "Invoices.pdf",
                ContentType = "application/pdf"
            });
        }

        Directory.Delete(Path.Combine(Path.GetTempPath(), "MergeTemp"), true);

        await loading.Hide();
    }
    public List<string> GeneratePdf(List<string> htmls)
    {
        List<string> pdfFilePaths = [];

        CustomAssemblyContext customAssembly = new();

        customAssembly.LoadDinkToPdfDll();

        for (int i = 0; i < htmls.Count; i++)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "MergeTemp");

                string htmlFilePath = Path.Combine(tempPath, $"outputHTML{i}.html");

                string pdfFilePath = Path.Combine(tempPath, $"output{i}.pdf");

                pdfFilePaths.Add(pdfFilePath);

                File.WriteAllText(htmlFilePath, htmls[i]);

                if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                var doc = new HtmlToPdfDocument
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = DinkToPdf.Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Out = pdfFilePath,
                        
                    },
                    Objects =
                    {
                        new ObjectSettings
                        {
                            Page = htmlFilePath,
                            LoadSettings = new LoadSettings { JSDelay = 5000 },
                            WebSettings = new WebSettings
                            {
                                EnableJavascript = true,
                                DefaultEncoding = "UTF-8",
                                LoadImages = true,
                               
                            }
                        }
                    }
                };

                Converter.Convert(doc);
                Console.WriteLine($"PDF generated successfully: {pdfFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during PDF generation: {ex.Message}");
            }
        }

        return pdfFilePaths;
    }
    public List<string> GenerateA6Pdf(List<string> htmls)
    {
        List<string> pdfFilePaths = new(); // List to store generated PDF file paths
        CustomAssemblyContext customAssembly = new();
        customAssembly.LoadDinkToPdfDll(); // Load the required DLL for PDF conversion

        string tempPath = Path.GetTempPath(); // Get the system's temporary folder path

        for (int i = 0; i < htmls.Count; i++) // Loop through each HTML string provided
        {
            try
            {
                // Parse the HTML content
                HtmlDocument htmlDoc = new();
                htmlDoc.LoadHtml(htmls[i]);

                // Check if <div class="PrintToolsBlock"> exists
                var printToolsBlockDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'PrintToolsBlock')]");
                if (printToolsBlockDiv != null)
                {
                    // Check if <div id="PrintBlock"> exists inside PrintToolsBlock
                    var printBlockDiv = printToolsBlockDiv.SelectSingleNode(".//div[@id='PrintBlock']");
                    if (printBlockDiv != null)
                    {
                        // Check if the number of <img> tags inside PrintBlock is more than 1
                        var imgTags = printBlockDiv.SelectNodes(".//img");
                        if (imgTags != null && imgTags.Count > 1)
                        {
                            // Retrieve the head and body nodes from the HTML document
                            var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
                            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                            // Store the original head content and body class
                            string originalHead = headNode?.OuterHtml ?? "<head></head>";
                            string originalBodyClass = bodyNode?.GetAttributeValue("class", string.Empty) ?? "";

                            for (int j = 0; j < imgTags.Count; j++) // Loop through each image tag
                            {
                                // Define paths for temporary HTML and PDF files
                                string htmlFilePath = Path.Combine(tempPath, $"outputHTML_{i}_{j}.html");
                                string pdfFilePath = Path.Combine(tempPath, $"output_{i}_{j}.pdf");

                                // Create new HTML content containing the image and the original structure
                                string newHtmlContent = $@"
                            <!DOCTYPE html>
                            <html>
                            {originalHead}
                            <body class='{originalBodyClass}'>
                                <div id='PrintBlock'>
                                        < div style ='position: relative; display: inline-block;'> <!-- Corrected display property -->
                                            { imgTags[j].OuterHtml}
                                    </ div >
                                </ div >
                            </ body >
                            </ html > ";
    
                            // Write the new HTML content to a temporary file
                            File.WriteAllText(htmlFilePath, newHtmlContent);

                                // Delete the existing PDF file if it already exists
                                if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                                // Create a PDF document configuration
                                var doc = new HtmlToPdfDocument
                                {
                                    GlobalSettings = new GlobalSettings
                                    {
                                        ColorMode = ColorMode.Color,
                                        Orientation = DinkToPdf.Orientation.Portrait,
                                        PaperSize = PaperKind.A6,
                                        Margins = new MarginSettings { Top = 3, Left = 0, Bottom = 5, Right = 0 },
                                        Out = pdfFilePath
                                    },
                                    Objects =
                                {
                                    new ObjectSettings
                                    {
                                        Page = htmlFilePath,
                                        LoadSettings = new LoadSettings { JSDelay = 5000 },
                                        WebSettings = new WebSettings
                                        {
                                            EnableJavascript = true,
                                            DefaultEncoding = "UTF-8",
                                            LoadImages = true
                                        }
                                    }
                                }
                                };

                                // Convert the HTML to PDF and add the PDF path to the list
                                Converter.Convert(doc);
                                pdfFilePaths.Add(pdfFilePath);
                                Console.WriteLine($"PDF generated successfully for PrintBlock image: {pdfFilePath}");
                            }
                            continue; // Skip to the next HTML input after processing images
                        }
                    }
                }

                // Existing logic for div_frame or PrintContent tables...
                var divFrames = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'div_frame')]");
                if (divFrames == null || divFrames.Count == 0) // Check if there are no div_frames
                {
                    // Check for <table class="PrintContent">
                    var printContentTables = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'PrintContent')]");
                    if (printContentTables != null && printContentTables.Count > 0)
                    {
                        // Define a temporary directory for merged content
                        tempPath = Path.Combine(tempPath, "MergeTemp");

                        // Retrieve the head and body nodes from the HTML document
                        var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
                        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        // Store the original head content and body class
                        string originalHead = headNode?.OuterHtml ?? "<head></head>";
                        string originalBodyClass = bodyNode?.GetAttributeValue("class", string.Empty) ?? "";

                        for (int j = 0; j < printContentTables.Count; j++) // Loop through each table
                        {
                            // Define paths for temporary HTML and PDF files
                            string htmlFilePath = Path.Combine(tempPath, $"outputHTML_{i}_{j}.html");
                            string pdfFilePath = Path.Combine(tempPath, $"output_{i}_{j}.pdf");

                            // Create new HTML content containing the table and the original structure
                            string newHtmlContent = $@"
                        <!DOCTYPE html>
                        <html>
                        {originalHead}
                        <body class='
                            {originalBodyClass}'>
                            <div style='position: relative; display: inline - block;'> <!-- Corrected display property -->
                                    { printContentTables[j].OuterHtml}
                            </ div >
                        </ body >
                        </ html > ";
    
                        // Create the temporary directory if it doesn't exist
                        Directory.CreateDirectory(tempPath);

                            // Write the new HTML content to a temporary file
                            File.WriteAllText(htmlFilePath, newHtmlContent);

                            // Delete the existing PDF file if it already exists
                            if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                            // Create a PDF document configuration
                            var doc = new HtmlToPdfDocument
                            {
                                GlobalSettings = new GlobalSettings
                                {
                                    ColorMode = ColorMode.Color,
                                    Orientation = DinkToPdf.Orientation.Portrait,
                                    PaperSize = PaperKind.A6,
                                    Margins = new MarginSettings { Top = 3, Left = 0, Bottom = 5, Right = 0 },
                                    Out = pdfFilePath
                                },
                                Objects =
                            {
                                new ObjectSettings
                                {
                                    Page = htmlFilePath,
                                    LoadSettings = new LoadSettings { JSDelay = 5000 },
                                    WebSettings = new WebSettings
                                    {
                                        EnableJavascript = true,
                                        DefaultEncoding = "UTF-8",
                                        LoadImages = true
                                    }
                                }
                            }
                            };

                            // Convert the HTML to PDF and add the PDF path to the list
                            Converter.Convert(doc);
                            pdfFilePaths.Add(pdfFilePath);
                            Console.WriteLine($"PDF generated successfully for PrintContent: {pdfFilePath}");
                        }
                    }
                    else // Process any remaining HTML without specific blocks or tables
                    {
                        // Define paths for temporary HTML and PDF files
                        string htmlFilePath = Path.Combine(tempPath, $"outputHTML{i}.html");
                        string pdfFilePath = Path.Combine(tempPath, $"output{i}.pdf");

                        // Add the PDF file path to the list
                        pdfFilePaths.Add(pdfFilePath);

                        // Write the original HTML content to a temporary file
                        File.WriteAllText(htmlFilePath, htmls[i]);

                        // Delete the existing PDF file if it already exists
                        if (File.Exists(pdfFilePath)) File.Delete(pdfFilePath);

                        // Create a PDF document configuration
                        var doc = new HtmlToPdfDocument
                        {
                            GlobalSettings = new GlobalSettings
                            {
                                ColorMode = ColorMode.Color,
                                Orientation = DinkToPdf.Orientation.Portrait,
                                PaperSize = PaperKind.A4,
                                Out = pdfFilePath
                            },
                            Objects =
                        {
                            new ObjectSettings
                            {
                                Page = htmlFilePath,
                                LoadSettings = new LoadSettings { JSDelay = 5000 },
                                WebSettings = new WebSettings
                                {
                                    EnableJavascript = true,
                                    DefaultEncoding = "UTF-8",
                                    LoadImages = true
                                }
                            }
                        }
                        };

                        // Convert the HTML to PDF
                        Converter.Convert(doc);
                        Console.WriteLine($"PDF generated successfully: {pdfFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during PDF generation
                Console.WriteLine($"Error generating PDF: {ex.Message}");
            }
        }

        // Return the list of generated PDF file paths
        return pdfFilePaths;
    }



    public MemoryStream CombinePdf(List<string> inputPdfPaths)
    {
        var memoryStream = new MemoryStream();

        string tempPath = Path.Combine(Path.GetTempPath(), "MergeTemp");

        string outputPdfPath = Path.Combine(tempPath, "combinedPdf.pdf");

        using (var outputDocument = new PdfDocument())
        {
            foreach (var path in inputPdfPaths)
            {
                var inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                foreach (var page in inputDocument.Pages)
                {
                    outputDocument.AddPage(page);
                }
            }

            outputDocument.Save(memoryStream);

            Console.WriteLine($"Combined PDF created successfully at: {outputPdfPath}");
        }

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task OnGenerateDeliveryNumber(MouseEventArgs e)
    {
        try
        {
            await loading.Show();

            List<Guid> orderIds = [.. Orders.Where(w => w.IsSelected).Select(s => s.OrderId)];

            List<Dictionary<string, string>> responseResults = []; string wholeErrorMessage = string.Empty;

            foreach (Guid orderId in orderIds)
            {
                OrderDto order = await _orderAppService.GetAsync(orderId);

                List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(orderId);

                foreach (OrderDeliveryDto orderDelivery in orderDeliveries)
                {
                    if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.PostOffice ||
                        orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.BlackCat1 ||
                        orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.BlackCatFrozen ||
                        orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.BlackCatFreeze)
                    {
                        ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id);

                        if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                    }

                    else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToEleven1 ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMart1 ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenFrozen)
                    {
                        ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(orderId, orderDelivery.Id);

                        if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                    }

                    else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                    {
                        PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id);

                        if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                    }

                    else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                    {
                        PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id);

                        if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                    }

                    else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.DeliveredByStore)
                    {
                        LogisticProviders? logisticProvider = null; DeliveryMethod? deliveryMethod = null; ItemStorageTemperature? temperature = null;

                        List<DeliveryTemperatureCostDto> deliveryTemperatureCosts = await _DeliveryTemperatureCostAppService.GetListAsync();

                        foreach (DeliveryTemperatureCostDto entity in deliveryTemperatureCosts)
                        {
                            if (orderDelivery.Items.Any(a => a.DeliveryTemperature == entity.Temperature))
                            {
                                logisticProvider = entity.LogisticProvider;

                                deliveryMethod = entity.DeliveryMethod;

                                temperature = entity.Temperature;
                            }
                        }

                        if (temperature is ItemStorageTemperature.Normal)
                        {
                            if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.FamilyMart1 ||
                                logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.SevenToEleven1 ||
                                logisticProvider is LogisticProviders.GreenWorldLogisticsC2C && deliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C ||
                                logisticProvider is LogisticProviders.GreenWorldLogisticsC2C && deliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C)
                            {
                                ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                            }

                            else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.PostOffice ||
                                     logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.BlackCat1)
                            {
                                ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                            }

                            else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal)
                            {
                                PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                            }

                            else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal)
                            {
                                PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                            }
                        }

                        else if (temperature is ItemStorageTemperature.Freeze)
                        {
                            if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.BlackCatFreeze)
                            {
                                ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                            }

                            else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze)
                            {
                                PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                            }

                            else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze)
                            {
                                PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                            }
                        }

                        else if (temperature is ItemStorageTemperature.Frozen)
                        {
                            if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                            {
                                PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                            }

                            else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                            {
                                PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                            }

                            else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.BlackCatFrozen)
                            {
                                ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                            }

                            else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.SevenToElevenFrozen)
                            {
                                ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                                if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                            }
                        }
                    }

                    else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup ||
                             orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery)
                    {
                        await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForSelfPickupAndHomeDeliveryAsync(orderId, orderDelivery.Id);
                    }
                }
            }

            if (responseResults is { Count: > 0 })
            {
                foreach (Dictionary<string, string> response in responseResults)
                {
                    if (wholeErrorMessage.IsNullOrEmpty())
                        wholeErrorMessage = wholeErrorMessage.Insert(0, response.Keys.First() + " -> " + response.Values.First());

                    else
                        wholeErrorMessage = wholeErrorMessage.Insert(wholeErrorMessage.Length + 1, Environment.NewLine + response.Keys.First() + " -> " + response.Values.First());
                }

                await loading.Hide();

                await _uiMessageService.Error(wholeErrorMessage);

                return;
            }

            await loading.Hide();

            if (SelectedTabName is "All") await UpdateItemList();

            else await LoadTabAsPerNameAsync(SelectedTabName);
        }
        catch (Exception ex)
        {
            await loading.Hide();

            await _uiMessageService.Error(ex.Message);
        }
    }

    public void AddToDictionary(List<Dictionary<string, string>> dictionaryList, string key, string value)
    {
        Dictionary<string, string> keyValuePairs = [];

        keyValuePairs.Add(key, value);

        dictionaryList.Add(keyValuePairs);
    }

    public async Task OnOrderDeliveryDataReadAsync(DataGridReadDataEventArgs<OrderDeliveryDto> e, Guid orderId)
    {
        await loading.Show();
        OrderDeliveries = [];

        if (!OrderDeliveriesByOrderId.ContainsKey(orderId))
        {
            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(orderId);

            OrderDeliveriesByOrderId[orderId] = orderDeliveries;
        }

        StateHasChanged();
        await loading.Hide();
    }

    public List<OrderDeliveryDto> GetOrderDeliveries(Guid orderId)
    {
        return OrderDeliveriesByOrderId.ContainsKey(orderId) ? OrderDeliveriesByOrderId[orderId] : [];
    }

    public async Task OnTabLoadDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e, string tabName)
    {
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "mySelectElement");
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "shippingMethodSelectElem");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "startDate");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "endDate");

        PageIndex = e.Page - 1;

        await LoadTabAsPerNameAsync(tabName);

        await GetGroupBuyList();

        await InvokeAsync(StateHasChanged);
    }

    private async Task GetGroupBuyList()
    {
        await loading.Show();
        GroupBuyList = await _groupBuyAppService.GetGroupBuyLookupAsync();
        await loading.Hide();

    }

    private async Task UpdateItemList()
    {
        try
        {
            await loading.Show();
            int skipCount = PageIndex * PageSize;
            var result = await _orderAppService.GetListAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                GroupBuyId = GroupBuyFilter,
                StartDate = StartDate,
                EndDate = EndDate,
                DeliveryMethod = DeliveryMethod
            });
            Orders = result?.Items.ToList() ?? new List<OrderDto>();
            TotalCount = (int?)result?.TotalCount ?? 0;

            await loading.Hide();
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.GetType().ToString());
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task OnShippingMethodSelectChangeAsync(string e)
    {
        if (!e.IsNullOrWhiteSpace() && !e.IsNullOrEmpty()) DeliveryMethod = Enum.Parse<DeliveryMethod>(e);

        else DeliveryMethod = null;

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);
    }

    public async Task LoadTabAsPerNameAsync(string tabName)
    {
        try
        {
            await loading.Show();

            SelectedOrder = null;

            int skipCount = PageIndex * PageSize;

            PagedResultDto<OrderDto> result = await _orderAppService.GetListAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                GroupBuyId = GroupBuyFilter,
                StartDate = StartDate,
                EndDate = EndDate,
                ShippingStatus = Enum.Parse<ShippingStatus>(tabName),
                DeliveryMethod = DeliveryMethod
            });

            Orders = [];

            Orders = [.. result.Items];

            TotalCount = (int?)result?.TotalCount ?? 0;

            await loading.Hide();
        }
        catch (Exception ex)
        {
            await loading.Hide();

            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    public void OnCheckboxValueChanged(bool e, OrderDto order)
    {
        order.IsSelected = e;

        if (SelectedTabName is not "ToBeShipped") return;

        if (order.IsSelected) OrdersSelected.Add(order);

        else OrdersSelected.Remove(order);

        NormalCount = OrdersSelected.Any(a => a.OrderItems.Any(ai => ai.DeliveryTemperature is ItemStorageTemperature.Normal)) ? NormalCount + 1 : NormalCount;

        FreezeCount = OrdersSelected.Any(a => a.OrderItems.Any(ai => ai.DeliveryTemperature is ItemStorageTemperature.Freeze)) ? FreezeCount + 1 : FreezeCount;

        FrozenCount = OrdersSelected.Any(a => a.OrderItems.Any(ai => ai.DeliveryTemperature is ItemStorageTemperature.Frozen)) ? FrozenCount + 1 : FrozenCount;
    }

    async Task OnSearch(Guid? e = null)
    {
        if (e == Guid.Empty) GroupBuyFilter = null;

        else GroupBuyFilter = e;

        PageIndex = 0;

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);
    }

    void HandleSelectAllChange(ChangeEventArgs e)
    {
        IsAllSelected = e.Value != null ? (bool)e.Value : false;
        Orders.ForEach(item =>
        {
            item.IsSelected = IsAllSelected;
        });
        StateHasChanged();
    }

    public async void NavigateToOrderDetails(DataGridRowMouseEventArgs<OrderDto> e)
    {
        await loading.Show();

        var id = e.Item.OrderId;
        NavigationManager.NavigateTo($"Orders/OrderDetails/{id}");

        await loading.Hide();
    }

    bool ShowCombineButton()
    {
        var selectedOrders = Orders.Where(x => x.IsSelected).ToList();

        if (selectedOrders.Count > 1)
        {
            var firstSelectedOrder = selectedOrders.First();
            bool allMatch = true;

            foreach (var order in selectedOrders)
            {
                if (order.GroupBuyId != firstSelectedOrder.GroupBuyId ||
                     order.CustomerName != firstSelectedOrder.CustomerName ||
                    order.CustomerEmail != firstSelectedOrder.CustomerEmail ||
                   order.ShippingStatus != firstSelectedOrder.ShippingStatus ||


                    order.OrderType != null
                    || order.ShippingStatus == ShippingStatus.Shipped
                    || order.ShippingStatus == ShippingStatus.Completed
                    || order.ShippingStatus == ShippingStatus.Closed)
                {
                    // If any property doesn't match, set allMatch to false and break the loop
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                // All selected orders have the same values for the specified properties
                Console.WriteLine("All selected orders have the same values.");
                return true;
            }
            else
            {
                // Not all selected orders have the same values for the specified properties
                Console.WriteLine("Selected orders have different values for the specified properties.");
                return false;
            }
        }
        else
        {
            // No selected orders found
            Console.WriteLine("No selected orders found.");
            return false;
        }


    }

    async void OnSortChange(DataGridSortChangedEventArgs e)
    {
        Sorting = e.FieldName + " " + (e.SortDirection != Blazorise.SortDirection.Default ? e.SortDirection : "");
        await UpdateItemList();
    }

    void ToggleRow(DataGridRowMouseEventArgs<OrderDto> e)
    {
        if (ExpandedOrderId == e.Item.OrderId) ExpandedOrderId = null;

        else ExpandedOrderId = e.Item.OrderId;

        if (ExpandedRows.Contains(e.Item.OrderId))
        {
            ExpandedRows.Remove(e.Item.OrderId);
        }
        else
        {
            ExpandedRows.Add(e.Item.OrderId);
        }
    }

    public async Task PrepareShipmentCheckboxChanged(bool e, OrderDto order)
    {
        order.IsSelected = e;

        if (order.DeliveryMethod is EnumValues.DeliveryMethod.SelfPickup || 
            order.DeliveryMethod is EnumValues.DeliveryMethod.HomeDelivery)
        {
            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(order.OrderId);

            IsDeliveryNoExists = orderDeliveries.Count == orderDeliveries.Count(c => c.DeliveryNo != null);
        }

        StateHasChanged();
    }

    private bool IsRowExpanded(OrderDto order)
    {
        return ExpandedOrderId == order.OrderId;
    }

    private async void MergeOrders()
    {
        var orderIds = Orders.Where(x => x.IsSelected).Select(x => x.OrderId).ToList();
        await _orderAppService.MergeOrdersAsync(orderIds);
        await UpdateItemList();
    }

    public async Task NavigateToOrderPrint()
    {
        List<OrderDto> selectedOrders = [.. Orders.Where(w => w.IsSelected)];

        List<Guid> selectedIds = [.. selectedOrders.Select(s => s.OrderId)];

        string selectedIdsStr = JsonConvert.SerializeObject(selectedIds);

        await JSRuntime.InvokeVoidAsync("openInNewTab", $"Orders/OrderShippingDetails/{selectedIdsStr}");
    }

    public async Task ShippingStatusChange()
    {
        await loading.Show();

        List<Guid> orderIds = [.. Orders.Where(w => w.IsSelected).Select(s => s.OrderId)];

        foreach (Guid orderId in orderIds)
        {
            await _OrderDeliveryAppService.ChangeShippingStatus(orderId);
        }

        await loading.Hide();

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);
    }

    private async void OrderItemShipped()
    {
        await loading.Show();
        var selectedOrders = OrdersSelected;
        foreach (var selectedOrder in selectedOrders)
        {
            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(selectedOrder.OrderId);
            if (orderDeliveries is { Count: > 0 })
            {
                foreach(var orderDelivery in orderDeliveries)
                    await _OrderDeliveryAppService.UpdateOrderDeliveryStatus(orderDelivery.Id);
            }
        }

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);

        await InvokeAsync(StateHasChanged);

        await loading.Hide();
    }

    private async void OrderItemDelivered()
    {
        await loading.Show();

        List<Guid> orderIds = [.. Orders.Where(w => w.IsSelected).Select(s => s.OrderId)];

        foreach (Guid orderId in orderIds)
        {
            await _OrderDeliveryAppService.UpdateDeliveredStatus(orderId);
        }

        await loading.Hide();

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);
    }

    private async void OrderItemPickedUp()
    {
        await loading.Show();

        List<Guid> orderIds = [.. Orders.Where(w => w.IsSelected).Select(s => s.OrderId)];

        foreach (Guid orderId in orderIds)
        {
            await _OrderDeliveryAppService.UpdatePickedUpStatus(orderId);
        }

        await loading.Hide();

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);
    }

    public async void IssueInvoice()
    {
        try
        {
            await loading.Show();
            var selectedOrder = Orders.SingleOrDefault(x => x.IsSelected);
            await _electronicInvoiceAppService.CreateInvoiceAsync(selectedOrder.OrderId);
            await loading.Hide();
            await _uiMessageService.Success(L["InvoiceIssueSuccessfully"]);
            await UpdateItemList();


        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.Message.ToString());


        }

    }

    async Task DownloadExcel()
    {
        try
        {
            int skipCount = PageIndex * PageSize;
            var orderIds = Orders.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            Sorting = Sorting != null ? Sorting : "OrderNo Ascending";

            var remoteStreamContent = await _orderAppService.GetListAsExcelFileAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                OrderIds = orderIds,
            });

            using (var responseStream = remoteStreamContent.GetStream())
            {
                // Create Excel file from the stream
                using (var memoryStream = new MemoryStream())
                {
                    await responseStream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Convert MemoryStream to byte array
                    var excelData = memoryStream.ToArray();

                    // Trigger the download using JavaScript interop
                    await JSRuntime.InvokeVoidAsync("downloadFile", new
                    {
                        ByteArray = excelData,
                        FileName = "ReconciliationStatement.xlsx",
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    });
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion
}
