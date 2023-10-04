using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.GroupBuys;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class GroupBuyList
    {
        public List<GroupBuyDto> GroupBuyListItem { get; set; }
        public bool IsAllSelected { get; private set; } = false;

        private readonly IUiMessageService _uiMessageService;

        private readonly IGroupBuyAppService _groupBuyAppService;
        int _pageIndex = 1;
        int _pageSize = 10;
        int Total = 0;
        private string Sorting = nameof(GroupBuy.GroupBuyName);

        public GroupBuyList(
            IGroupBuyAppService groupBuyAppService,
            IUiMessageService messageService
            )
        {
            _groupBuyAppService = groupBuyAppService;
            _uiMessageService = messageService;
            GroupBuyListItem = new List<GroupBuyDto>();
        }

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
                var result = await _groupBuyAppService.GetListAsync(new GetGroupBuyInput
                {
                    Sorting = Sorting,
                    MaxResultCount = _pageSize,
                    SkipCount = skipCount
                });
                GroupBuyListItem = result.Items.ToList();
                Total = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
                Console.WriteLine(ex.ToString());
            }
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
                        await _groupBuyAppService.DeleteManyGroupBuyItemsAsync(groupBuyItemsids);
                        await UpdateGroupBuyList();
                        IsAllSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<GroupBuyDto> e)
        {
            _pageIndex = e.Page - 1;
            await UpdateGroupBuyList();
            await InvokeAsync(StateHasChanged);
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
            Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateGroupBuyList();
        }
    }
}
