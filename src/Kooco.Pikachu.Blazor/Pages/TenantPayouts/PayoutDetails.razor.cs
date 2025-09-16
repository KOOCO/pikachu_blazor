using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.Blazor.Pages.TenantPayouts.Steps.PayoutDetails;
using Kooco.Pikachu.TenantPayouts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class PayoutDetails
{
    [CascadingParameter] public TenantPayoutContext Context { get; set; } = default!;
    private GetTenantPayoutRecordListDto Filters { get; set; }
    private TenantPayoutDetailSummaryDto Summary { get; set; } = new();
    private IReadOnlyList<TenantPayoutRecordDto> PayoutDetailList { get; set; } = [];
    private List<TenantPayoutRecordDto> SelectedPayoutDetails { get; set; } = [];
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private bool AnySelected => SelectedPayoutDetails is { Count: > 0 };
    private int SelectedCount => SelectedPayoutDetails?.Count ?? 0;
    private string SelectedCountText => AnySelected ? string.Format("({0})", SelectedCount) : "";

    async Task ApplyFiltersAsync()
    {
        if (Filters?.StartDate.Year != Context.Year || Filters?.EndDate.Year != Context.Year)
        {
            return;
        }

        CurrentPage = 1;

        await LoadData();

        Context.Filtering = false;
        Context.Resetting = false;
        await InvokeAsync(StateHasChanged);
    }

    async Task LoadData()
    {
        SelectedPayoutDetails = [];
        await GetSummaryAsync();
        await GetPayoutRecordsAsync();
    }

    async Task GetSummaryAsync()
    {
        try
        {
            Summary = await Context.Service.GetTenantPayoutDetailSummaryAsync(Filters);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task GetPayoutRecordsAsync()
    {
        try
        {
            var result = await Context.Service.GetListAsync(
                new GetTenantPayoutRecordListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    TenantId = Filters.TenantId,
                    FeeType = Filters.FeeType,
                    StartDate = Filters.StartDate,
                    EndDate = Filters.EndDate,
                    PaymentMethod = Filters.PaymentMethod
                }
            );

            TotalCount = (int)result.TotalCount;
            PayoutDetailList = result.Items;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TenantPayoutRecordDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetPayoutRecordsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private static bool RowSelectableHandler(RowSelectableEventArgs<TenantPayoutRecordDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;

    async Task MarkAsPaidAsync(TenantPayoutRecordDto record)
    {
        await MarkAsPaidAsync([record]);
    }

    async Task MarkAsPaidAsync(List<TenantPayoutRecordDto> records)
    {
        try
        {
            var ids = records.Select(r => r.Id).ToList();
            records.ForEach(r => r.MarkingPaid = true);
            await InvokeAsync(StateHasChanged);
            await Context.Service.MarkAsPaidAsync(ids);
            await LoadData();
        }
        catch (Exception ex)
        {
            records.ForEach(r => r.MarkingPaid = false);
            await HandleErrorAsync(ex);
        }

        await InvokeAsync(StateHasChanged);
    }

    async Task TransferToWalletAsync(TenantPayoutRecordDto record)
    {
        await TransferToWalletAsync([record]);
    }

    async Task TransferToWalletAsync(List<TenantPayoutRecordDto> records)
    {
        try
        {
            var ids = records.Select(r => r.Id).ToList();
            records.ForEach(r => r.Transferring = true);
            await InvokeAsync(StateHasChanged);
            var result = await Context.Service.TransferToWalletAsync(ids);
            if (result == -1)
            {
                await Message.Warn(L["SomeRecordsWereNotPaidDueToMissingWallet"]);
            }
            await LoadData();
        }
        catch (Exception ex)
        {
            records.ForEach(r => r.Transferring = false);
            await HandleErrorAsync(ex);
        }

        await InvokeAsync(StateHasChanged);
    }

    async Task ExportAsync(PayoutExportOptionsArgs args)
    {
        try
        {
            Filters.ExportCurrent = args.ExportCurrent;
            var bytes = await Context.Service.ExportAsync(Filters, args.ExportType, SelectedPayoutDetails);

            if (bytes != null)
            {
                string fileName = $"{L["PayoutDetails"]}_";
                if (SelectedPayoutDetails is { Count: > 0 })
                {
                    fileName += $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
                }
                else
                {
                    if (Filters.ExportCurrent)
                    {
                        fileName += $"{Filters.StartDate:yyyy_MM_dd}_To_{Filters.EndDate:yyyy_MM_dd}";
                    }
                    else
                    {
                        fileName += $"{new DateTime(Filters.Year, 1, 1):yyyy_MM_dd}_To_{new DateTime(Filters.Year, 12, 31):yyyy_MM_dd}";
                    }
                }

                if (args.ExportType == PayoutExportOptions.Excel)
                {
                    fileName += ".xlsx";
                    await ExcelDownloadHelper.DownloadExcelAsync(bytes, fileName);
                }
                if (args.ExportType == PayoutExportOptions.Csv)
                {
                    fileName += ".csv";
                    await ExcelDownloadHelper.DownloadCsvAsync(bytes, fileName);
                }
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }

        Context.Exporting = false;
        await InvokeAsync(StateHasChanged);
    }
}