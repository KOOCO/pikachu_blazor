using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.TenantPaymentFees;
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
    [Parameter] public ITenantPayoutAppService Service { get; set; }
    [Parameter] public Guid? TenantId { get; set; }
    [Parameter] public PaymentFeeType? FeeType { get; set; }
    [Parameter] public int? Year { get; set; }

    private GetTenantPayoutRecordListDto Filters { get; set; } = new();
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
    private bool Filtering { get; set; }
    private bool Resetting { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                //await ResetAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    async Task ApplyFiltersAsync()
    {
        Filtering = true;
        StateHasChanged();

        CurrentPage = 1;

        await GetSummaryAsync();
        await GetPayoutRecordsAsync();

        Filtering = false;
        StateHasChanged();
    }

    //async Task ResetAsync()
    //{
    //    Resetting = true;
    //    StateHasChanged();

    //    Year ??= DateTime.Now.Year;

    //    Filters = new()
    //    {
    //        TenantId = TenantId!.Value,
    //        FeeType = FeeType!.Value,
    //        StartDate = new DateTime(Year.Value, 1, 1),
    //        EndDate = new DateTime(Year.Value, 12, 31)
    //    };

    //    await GetSummaryAsync();
    //    await GetPayoutRecordsAsync();
    //    Resetting = false;
    //    StateHasChanged();
    //}

    async Task GetSummaryAsync()
    {
        try
        {
            Summary = await Service.GetTenantPayoutDetailSummaryAsync(Filters);
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
            var result = await Service.GetListAsync(
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
}