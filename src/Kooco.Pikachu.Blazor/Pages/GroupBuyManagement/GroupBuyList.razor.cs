using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.GroupBuys;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
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
        private LoadingIndicator loading { get; set; } = new();

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

        private async Task CopyAsync() {

            var id = GroupBuyListItem.Where(x => x.IsSelected == true).Select(x => x.Id).FirstOrDefault();

            await _groupBuyAppService.CopyAsync(id);
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
                await _uiMessageService.Error(ex.GetType()?.ToString());
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
    }
}
