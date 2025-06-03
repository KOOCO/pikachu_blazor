using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Members.MemberTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Members.MemberTags;

public partial class MemberTags
{
    private IReadOnlyList<MemberTagDto> MemberTagList { get; set; } = [];
    private string Filter { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private bool IsDeleting { get; set; }
    private List<MemberTagDto> SelectedMemberTags { get; set; } = [];
    private MemberTagsModal TagModal { get; set; }

    private async Task GetMemberTagsAsync()
    {
        try
        {
            var result = await MemberTagAppService.GetListAsync(
                new GetMemberTagsListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filter
                }
            );

            MemberTagList = result.Items;
            TotalCount = (int)result.TotalCount;
            SelectedMemberTags = [];
        }
        catch (Exception ex)
        {

            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MemberTagDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetMemberTagsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task OnCheckedChanged(MemberTagDto memberTag, bool value)
    {
        try
        {
            await MemberTagAppService.SetIsEnabledAsync(memberTag.Name, value);
            await GetMemberTagsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task DeleteAsync()
    {
        if (SelectedMemberTags?.Count > 0)
        {
            var confirm = await Message.Confirm(L["AreYouSureToDeleteSelectedMemberTags"]);
            if (confirm)
            {
                try
                {
                    IsDeleting = true;
                    StateHasChanged();

                    var tagsToDelete = SelectedMemberTags.Select(tag => tag.Name).ToList();
                    await MemberTagAppService.DeleteManyAsync(tagsToDelete);
                    IsDeleting = false;

                    await GetMemberTagsAsync();
                }
                catch (Exception ex)
                {
                    IsDeleting = false;
                    await HandleErrorAsync(ex);
                }
            }
        }
    }

    async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetMemberTagsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task Edit(MemberTagDto context)
    {
        await TagModal.Edit(context.Id, context.Name);
    }

    private static bool RowSelectableHandler(RowSelectableEventArgs<MemberTagDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
}