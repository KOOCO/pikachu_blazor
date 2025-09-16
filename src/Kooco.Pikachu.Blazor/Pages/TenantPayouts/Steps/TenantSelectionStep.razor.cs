using Kooco.Pikachu.TenantPayouts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts.Steps;

public partial class TenantSelectionStep
{
    [CascadingParameter] public TenantPayoutContext Context { get; set; } = default!;
    [Parameter] public EventCallback<TenantPayoutSummaryDto?> TenantChanged { get; set; }

    private GetTenantSummariesDto Filters { get; set; } = new();
    private IReadOnlyList<TenantPayoutSummaryDto> Tenants { get; set; } = [];
    private TenantPayoutSummaryDto? Tenant { get; set; }

    private int PageSize { get; set; } = PagedAndSortedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private bool Loading { get; set; } = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await GetData();
    }

    async Task GetData()
    {
        try
        {
            Filters.SkipCount = (CurrentPage - 1) * PageSize;
            var result = await Context.Service.GetTenantSummariesAsync(Filters);
            Tenants = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex) { await HandleErrorAsync(ex); }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    private async Task SelectTenant(TenantPayoutSummaryDto? value)
    {
        if (Tenant != value)
        {
            Tenant = value;
            await TenantChanged.InvokeAsync(value);
        }
    }

    async Task OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await Search();
        }
    }

    async Task Refresh()
    {
        CurrentPage = 1;
        await GetData();
    }

    async Task Search()
    {
        CurrentPage = 1;
        await GetData();
    }

    async Task OnPageChange(AntDesign.PaginationEventArgs args)
    {
        CurrentPage = args.Page;
        await GetData();
    }
}