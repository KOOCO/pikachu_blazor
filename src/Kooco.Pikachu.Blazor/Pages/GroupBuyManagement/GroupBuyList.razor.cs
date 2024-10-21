using Blazorise;
using Blazorise.Components;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class GroupBuyList
{
    #region Inject
    public List<GroupBuyDto> GroupBuyListItem { get; set; }
    public bool IsAllSelected { get; private set; } = false;

    private readonly IUiMessageService _uiMessageService;

    private readonly IGroupBuyAppService _groupBuyAppService;
    int _pageIndex = 1;
    int _pageSize = 10;
    int Total = 0;
    public DateTime? CreatedDate { get; set; }
    private string Sorting = nameof(GroupBuy.GroupBuyName);
    private LoadingIndicator loading { get; set; } = new();
    private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
    private string? SelectedAutoCompleteText { get; set; }
    private List<KeyValueDto> ItemsList { get; set; } = new();
    #endregion

    #region Constructor
    public GroupBuyList(
        IGroupBuyAppService groupBuyAppService,
        IUiMessageService messageService
    )
    {
        _groupBuyAppService = groupBuyAppService;
        _uiMessageService = messageService;
        GroupBuyListItem = new List<GroupBuyDto>();
    }
    #endregion

    #region Methods
    private void HandleSelectAllChange(ChangeEventArgs e)
    {
        IsAllSelected = (bool?)e.Value ?? false;
        GroupBuyListItem.ForEach(item =>
        {
            item.IsSelected = IsAllSelected;
        });
        StateHasChanged();
       
    }

    private async Task UpdateGroupBuyList()
    {
        try
        {
            int skipCount = _pageIndex * _pageSize;

            PagedResultDto<GroupBuyDto> result = await _groupBuyAppService.GetListAsync(new GetGroupBuyInput
            {
                Sorting = Sorting,
                MaxResultCount = _pageSize,
                SkipCount = skipCount,
                FilterText= SelectedAutoCompleteText
            });

            GroupBuyListItem = [.. result.Items];

            foreach (GroupBuyDto groupBuyItem in GroupBuyListItem)
            {
                groupBuyItem.EntryURL = $"{(await MyTenantAppService.FindTenantDomainAsync(CurrentTenant.Id))?.TrimEnd('/')}/{groupBuyItem.Id}";
            }

            Total = (int)result.TotalCount;
            //ItemsLookup = await _groupBuyAppService.GetAllGroupBuyLookupAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType()?.ToString());
            Console.WriteLine(ex.ToString());
        }
    }

    private async Task CopyAsync() {

        var id = GroupBuyListItem.Where(x => x.IsSelected == true).Select(x => x.Id).FirstOrDefault();

       var copy= await _groupBuyAppService.CopyAsync(id);
        NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList/Edit/" + copy.Id);


    }
    async Task OnSearch()
    {
        _pageIndex = 0;
        await UpdateGroupBuyList();
    }
    private async Task DeleteSelectedAsync()
    {
        try
        {
            var groupBuyItemsids = GroupBuyListItem.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (groupBuyItemsids.Count > 0)
            {
                var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteSelectedItem"]);
                if (confirmed)
                {
                    await loading.Show();
                    await _groupBuyAppService.DeleteManyGroupBuyItemsAsync(groupBuyItemsids);
                    await UpdateGroupBuyList();
                    IsAllSelected = false;
                    await loading.Hide();
                }
            }
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.Message.ToString());
            Console.WriteLine(ex.ToString());
        }
    }
    public async Task OnGroupBuyAvaliablityChanged(Guid id)
    {
        try
        {
            await loading.Show();
            var freebie = GroupBuyListItem.Where(x => x.Id == id).First();
            freebie.IsGroupBuyAvaliable = !freebie.IsGroupBuyAvaliable;
            await _groupBuyAppService.ChangeGroupBuyAvailability(id);
            await UpdateGroupBuyList();
            await InvokeAsync(StateHasChanged);
            await loading.Hide();
        }
        catch (BusinessException ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.Code.ToString());
            Console.WriteLine(ex.ToString());
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.GetType().ToString());
            Console.WriteLine(ex.ToString());
        }
    }
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<GroupBuyDto> e)
    {
        try
        {
            await loading.Show();
            _pageIndex = e.Page - 1;
            await UpdateGroupBuyList();
            await InvokeAsync(StateHasChanged);
            await loading.Hide();
        }
        catch (Exception ex)
        {
            await loading.Hide();
            Console.WriteLine(ex.ToString());
        }
    }

    public void CreateNewItem()
    {
        NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList/New");
    }

    public void OnEditItem(DataGridRowMouseEventArgs<GroupBuyDto> e)
    {
        var id = e.Item.Id;
        NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList/Edit/" + id);
    }

    async void OnSortChange(DataGridSortChangedEventArgs e)
    {
        await loading.Show();
        Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
        await UpdateGroupBuyList();
        await loading.Hide();
    }

    private async Task CopyLinkToClipboard(string? entryUrl)
    {
        if (entryUrl.IsNullOrEmpty()) return;

        string path = entryUrl;

        await _JSRuntime.InvokeVoidAsync("parent.navigator.clipboard.writeText", path);
    }
    #endregion
}
