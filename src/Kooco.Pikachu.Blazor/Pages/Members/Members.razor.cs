using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Members;

public partial class Members
{
    private IReadOnlyList<MemberDto> MembersList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }

    private GetMemberListDto Filters { get; set; }

    private MemberDto SelectedMember { get; set; }
    private List<MemberDto> SelectedMembers { get; set; }
    private bool FiltersVisible { get; set; } = false;
    private bool IsExporting { get; set; }

    public Members()
    {
        MembersList = [];
        Filters = new();
    }

    protected override async Task OnInitializedAsync()
    {
        await GetMembersAsync();
        await base.OnInitializedAsync();
    }

    private async Task GetMembersAsync()
    {
        try
        {
            var result = await MemberAppService.GetListAsync(
                new GetMemberListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    MemberType = Filters.MemberType
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

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MemberDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetMembersAsync();

        await InvokeAsync(StateHasChanged);
    }

    private void EditMember(MemberDto member)
    {
        NavigationManager.NavigateTo("/Members/Details/" + member.Id);
    }

    private async Task ExportAsync()
    {
        if (SelectedMembers?.Count > 0)
        {
            await ExcelDownloadHelper.DownloadExcelAsync(SelectedMembers, L["Members"] + ".xlsx");
        }
    }

    private async Task ExportAllAsync()
    {
        try
        {
            IsExporting = true;
            var allMembers = await MemberAppService.GetAllAsync();
            await ExcelDownloadHelper.DownloadExcelAsync(allMembers, L["Members"] + ".xlsx");
            IsExporting = false;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            IsExporting = false;
        }
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

    private static bool RowSelectableHandler(RowSelectableEventArgs<MemberDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
}