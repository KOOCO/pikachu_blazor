using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Members.MemberDetails;

public partial class MemberOrdersTab
{
    [Parameter]
    public MemberDto? Member { get; set; }

    [Parameter]
    public MemberOrderStatsDto? OrderStats { get; set; }

    private List<KeyValueDto> GroupBuysLookup { get; set; }
    private IReadOnlyList<OrderDto> MemberOrders { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }

    private GetOrderListDto OrderFilters { get; set; }
    private bool FiltersVisible { get; set; }

    public MemberOrdersTab()
    {
        MemberOrders = [];
        GroupBuysLookup = [];
        OrderFilters = new();
        OrderStats = new();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await GetMemberOrdersAsync();
                GroupBuysLookup = await MemberAppService.GetGroupBuyLookupAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetMemberOrdersAsync()
    {
        try
        {
            if (Member is null) return;
            var result = await MemberAppService.GetMemberOrdersAsync(
                new GetOrderListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = OrderFilters.Filter,
                    GroupBuyId = OrderFilters.GroupBuyId,
                    StartDate = OrderFilters.StartDate,
                    EndDate = OrderFilters.EndDate,
                    ShippingStatus = OrderFilters.ShippingStatus,
                    DeliveryMethod = OrderFilters.DeliveryMethod,
                    UserId = Member.Id
                }
            );

            MemberOrders = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetMemberOrdersAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetMemberOrdersAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        OrderFilters = new();

        await GetMemberOrdersAsync();

        await InvokeAsync(StateHasChanged);
    }
}