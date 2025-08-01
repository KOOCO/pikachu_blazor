﻿using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class GroupBuyExternalReportDetails
{
    [Parameter]
    public string Id { get; set; }
    private GroupBuyReportDetailsDto ReportDetails { get; set; }
    private List<OrderDto> Orders { get; set; } = [];
    private int TotalCount { get; set; }
    private OrderDto SelectedOrder { get; set; }
    private int PageIndex { get; set; } = 1;
    private int PageSize { get; set; } = 10;
    private string? Sorting { get; set; }
    private string? Filter { get; set; }
    private DateTime? StartDate { get; set; }
    private DateTime? EndDate { get; set; }
    private OrderStatus? OrderStatus { get; set; }

    private readonly HashSet<Guid> ExpandedRows = [];
    private LoadingIndicator Loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await GetReportDetailsAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
    {
        PageIndex = e.Page - 1;
        await UpdateItemList();
        await InvokeAsync(StateHasChanged);
    }

    async Task GetReportDetailsAsync()
    {
        ReportDetails = await _groupBuyAppService.GetGroupBuyReportDetailsAsync(Guid.Parse(Id), StartDate, EndDate, OrderStatus);
        StartDate = ReportDetails?.StartDate;
        EndDate = ReportDetails?.EndDate;
    }

    private async Task UpdateItemList()
    {
        try
        {
            await Loading.Show();
            int skipCount = PageIndex * PageSize;
            var result = await _orderAppService.GetListAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                GroupBuyId = Guid.Parse(Id),
                StartDate = StartDate,
                EndDate = EndDate,
                OrderStatus = OrderStatus
            }, true);
            Orders = result?.Items.ToList() ?? [];
            TotalCount = (int?)result?.TotalCount ?? 0;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }

    async void OnSortChange(DataGridSortChangedEventArgs e)
    {
        Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
        await UpdateItemList();
    }

    void ToggleRow(DataGridRowMouseEventArgs<OrderDto> e)
    {
        if (ExpandedRows.Contains(e.Item.Id))
        {
            ExpandedRows.Remove(e.Item.Id);
        }
        else
        {
            ExpandedRows.Add(e.Item.Id);
        }
    }

    async Task DownloadExcel()
    {
        try
        {
            await Loading.Show();
            var remoteStreamContent = await _groupBuyAppService.GetListAsExcelFileAsync(Guid.Parse(Id), StartDate, EndDate, OrderStatus, includeShippingFee: false);
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
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync(ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }

    async Task FilterAsync()
    {
        PageIndex = 0;
        await GetReportDetailsAsync();
        await UpdateItemList();
    }
}
