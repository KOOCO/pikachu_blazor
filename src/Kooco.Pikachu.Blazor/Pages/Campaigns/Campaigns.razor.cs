using Blazorise.DataGrid;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.TierManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using Kooco.Pikachu.Campaigns;
using System.Linq;
using Blazorise;

namespace Kooco.Pikachu.Blazor.Pages.Campaigns;

public partial class Campaigns
{
    private IReadOnlyList<CampaignDto> MembersList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private GetCampaignListDto Filters { get; set; }
    private bool FiltersVisible { get; set; } = false;

    public Campaigns()
    {
        MembersList = [];
        Filters = new();
    }

    protected override async Task OnInitializedAsync()
    {
        await GetMembersAsync();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {

            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    private async Task GetMembersAsync()
    {
        try
        {
            var result = await CampaignAppService.GetListAsync(
                new GetCampaignListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    IsEnabled = Filters.IsEnabled,
                    StartDate = Filters.StartDate,
                    EndDate = Filters.EndDate
                }
            );

            MembersList = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CampaignDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetMembersAsync();

        await InvokeAsync(StateHasChanged);
    }

    private void EditMember(CampaignDto member)
    {
        NavigationManager.NavigateTo("/Campaigns/Edit/" + member.Id);
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetMembersAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        Filters = new();

        await GetMembersAsync();

        await InvokeAsync(StateHasChanged);
    }
}