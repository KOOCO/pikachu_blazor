using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.UserCumulativeCredits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Members.MemberDetails;

public partial class MemberShoppingCreditsTab
{
    [Parameter]
    public MemberDto? Member { get; set; }
    public bool CanCreateShoppingCredits { get; set; }
    private UserCumulativeCreditDto CumulativeCredits { get; set; }
    private IReadOnlyList<MemberCreditRecordDto> MemberCreditRecords { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }

    private GetMemberCreditRecordListDto CreditRecordFilters { get; set; }
    private bool FiltersVisible { get; set; }

    public MemberShoppingCreditsTab()
    {
        CumulativeCredits = new();
        MemberCreditRecords = [];
        CreditRecordFilters = new();
    }

    protected override async Task OnInitializedAsync()
    {
        CanCreateShoppingCredits = await AuthorizationService.IsGrantedAsync(PikachuPermissions.UserShoppingCredits.Create);
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                CumulativeCredits = await MemberAppService.GetMemberCumulativeCreditsAsync(Member.Id);
                await GetMemberCreditRecordsAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetMemberCreditRecordsAsync()
    {
        try
        {
            if (Member is null) return;
            var result = await MemberAppService.GetMemberCreditRecordAsync(
                Member.Id,
                new GetMemberCreditRecordListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = CreditRecordFilters.Filter,
                    UsageTimeFrom = CreditRecordFilters.UsageTimeFrom,
                    UsageTimeTo = CreditRecordFilters.UsageTimeTo,
                    ExpiryDateFrom = CreditRecordFilters.ExpiryDateFrom,
                    ExpiryDateTo = CreditRecordFilters.ExpiryDateTo,
                    MinRemainingCredits = CreditRecordFilters.MinRemainingCredits,
                    MaxRemainingCredits = CreditRecordFilters.MaxRemainingCredits,
                    MinAmount = CreditRecordFilters.MinAmount,
                    MaxAmount = CreditRecordFilters.MaxAmount
                }
            );

            MemberCreditRecords = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MemberCreditRecordDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetMemberCreditRecordsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetMemberCreditRecordsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        CreditRecordFilters = new();

        await GetMemberCreditRecordsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private void GrantShoppingCredits()
    {
        if (Member is null || !CanCreateShoppingCredits) return;
        NavigationManager.NavigateTo("Members/ShoppingCredits/Grant/" + Member.Id);
    }
}