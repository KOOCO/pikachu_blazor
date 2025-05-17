using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.EdmManagement;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.EdmManagement;

public partial class EdmList
{
    private IReadOnlyList<EdmDto> EdmsList { get; set; } = [];
    private EdmDto Selected { get; set; } = new();
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private GetEdmListDto Filters { get; set; } = new();
    private bool FiltersVisible { get; set; }
    private bool CanCreateEdm { get; set; }
    private bool CanEditEdm { get; set; }
    private bool CanDeleteEdm { get; set; }
    private IReadOnlyList<EdmTemplateType> TemplateTypeOptions { get; set; } = Enum.GetValues<EdmTemplateType>();
    private IReadOnlyList<EdmMemberType> MemberTypeOptions { get; set; } = Enum.GetValues<EdmMemberType>();
    private IReadOnlyList<EdmSendFrequency> SendFrequencyOptions { get; set; } = Enum.GetValues<EdmSendFrequency>();
    private IReadOnlyList<KeyValueDto> CampaignOptions { get; set; } = [];
    private IReadOnlyList<KeyValueDto> GroupBuyOptions { get; set; } = [];
    private IReadOnlyList<string> MemberTagOptions { get; set; } = [];

    private Modal MemberTagsModal;
    private Modal GroupBuysModal;

    protected override async Task OnInitializedAsync()
    {
        await SetAuthorizationAsync();
        await GetEdmsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            GroupBuyOptions = await GroupBuyAppService.GetAllGroupBuyLookupAsync();
            CampaignOptions = await CampaignAppService.GetLookupAsync();
            MemberTagOptions = await MemberTagAppService.GetMemberTagNamesAsync();
        }
    }

    private async Task SetAuthorizationAsync()
    {
        CanCreateEdm = await AuthorizationService.IsGrantedAsync(PikachuPermissions.EdmManagement.Create);
        CanEditEdm = await AuthorizationService.IsGrantedAsync(PikachuPermissions.EdmManagement.Edit);
        CanDeleteEdm = await AuthorizationService.IsGrantedAsync(PikachuPermissions.EdmManagement.Delete);
    }

    private async Task GetEdmsAsync()
    {
        try
        {
            var result = await EdmAppService.GetListAsync(
                new GetEdmListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    TemplateType = Filters.TemplateType,
                    CampaignId = Filters.CampaignId,
                    MemberType = Filters.MemberType,
                    MemberTags = Filters.MemberTags,
                    ApplyToAllGroupBuys = Filters.ApplyToAllGroupBuys,
                    GroupBuyIds = Filters.GroupBuyIds,
                    StartDate = Filters.StartDate,
                    EndDate = Filters.EndDate,
                    MinSendTime = Filters.MinSendTime,
                    MaxSendTime = Filters.MaxSendTime,
                    SendFrequency = Filters.SendFrequency
                }
            );

            EdmsList = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EdmDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetEdmsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task DeleteAsync(EdmDto edm)
    {
        try
        {
            var confirm = await Message.Confirm(L["AreYouSureToDeleteEdm"]);
            if (confirm)
            {
                await EdmAppService.DeleteAsync(edm.Id);
                await GetEdmsAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    void Navigate(Guid? id = null)
    {
        NavigationManager.NavigateTo(id.HasValue ? "Edm/Edit/" + id : "Edm/Create");
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetEdmsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        Filters = new();

        await GetEdmsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task Copy(object? text)
    {
        await CopyService.Copy(text);
    }

    Task ShowMemberTagsModal(EdmDto edm)
    {
        Selected = edm;
        return MemberTagsModal.Show();
    }

    Task HideMemberTagsModal()
    {
        Selected = new();
        return MemberTagsModal.Hide();
    }

    Task ShowGroupBuysModal(EdmDto edm)
    {
        Selected = edm;
        return GroupBuysModal.Show();
    }

    Task HideGroupBuysModal()
    {
        Selected = new();
        return GroupBuysModal.Hide();
    }
}