using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Campaigns;

public partial class Campaigns
{
    private IReadOnlyList<CampaignDto> CampaignsList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private long ActiveCount { get; set; }
    private GetCampaignListDto Filters { get; set; }
    private bool FiltersVisible { get; set; } = false;
    private bool CanCreate { get; set; }
    private bool CanEdit { get; set; }
    private bool CanDelete { get; set; }

    public Campaigns()
    {
        CampaignsList = [];
        Filters = new();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetAuthorizationAsync();
        await GetCampaignsAsync();
        await base.OnInitializedAsync();
    }

    async Task SetAuthorizationAsync()
    {
        CanCreate = await AuthorizationService.IsGrantedAsync(PikachuPermissions.Campaigns.Create);
        CanEdit = await AuthorizationService.IsGrantedAsync(PikachuPermissions.Campaigns.Edit);
        CanDelete = await AuthorizationService.IsGrantedAsync(PikachuPermissions.Campaigns.Delete);
    }

    private async Task GetCampaignsAsync()
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

            CampaignsList = result.Items;
            TotalCount = (int)result.TotalCount;
            ActiveCount = await CampaignAppService.GetActiveCampaignsCountAsync();
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

        await GetCampaignsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private void Edit(CampaignDto member)
    {
        NavigationManager.NavigateTo("/Campaigns/Edit/" + member.Id);
    }

    async Task DeleteAsync(CampaignDto campaign)
    {
        try
        {
            var confirm = await Message.Confirm(L["AreYouSureToDeleteCampaign", campaign.Name]);
            if (confirm)
            {
                await CampaignAppService.DeleteAsync(campaign.Id);
                await GetCampaignsAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task SetIsEnabled(CampaignDto campaign)
    {
        try
        {
            var confirm = await Message.Confirm(L["AreYouSureToSetCampaignStatus", campaign.IsEnabled ? L["Inactive"] : L["Active"]]);
            if (confirm)
            {
                await CampaignAppService.SetIsEnabledAsync(campaign.Id, !campaign.IsEnabled);
                await GetCampaignsAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetCampaignsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        Filters = new();

        await GetCampaignsAsync();

        await InvokeAsync(StateHasChanged);
    }
}