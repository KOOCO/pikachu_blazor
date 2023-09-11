using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Items;
using System.Collections.Generic;
using Blazorise.DataGrid;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using System.Linq;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
using System;

namespace Kooco.Pikachu.Blazor.Pages.SetItem
{
    public partial class SetItem
    {
        public List<SetItemDto> SetItemList;
        public bool IsAllSelected = false;
        int PageIndex = 1;
        int PageSize = 10;
        int Total = 0;

        private readonly IUiMessageService _uiMessageService;
        private readonly IItemAppService _itemAppService;
        private readonly ISetItemAppService _setItemAppService;

        public SetItem(IItemAppService itemAppService, IUiMessageService messageService, ISetItemAppService setItemAppService)
        {
            _itemAppService = itemAppService;
            _uiMessageService = messageService;
            _setItemAppService = setItemAppService;
            SetItemList = new List<SetItemDto>();
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SetItemDto> e)
        {
            PageIndex = e.Page - 1;
            await UpdateItemList();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateItemList()
        {
            int skipCount = PageIndex * PageSize;
            var result = await _setItemAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                Sorting = nameof(SetItemDto.SetItemName),
                MaxResultCount = PageSize,
                SkipCount = skipCount
            });
            SetItemList = result.Items.ToList();
            Total = (int)result.TotalCount;
        }

        public void OnEditItem(DataGridRowMouseEventArgs<SetItemDto> e)
        {
            var id = e.Item.Id;
            NavigationManager.NavigateTo($"SetItem/Edit/{id}");
        }

        private void NavigateToCreateSetItem()
        {
            NavigationManager.NavigateTo("/SetItem/Create");
        }

        private void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = (bool)e.Value;
            SetItemList.ForEach(item =>
            {
                item.IsSelected = IsAllSelected;
            });
            StateHasChanged();
        }

        private async Task DeleteSelectedAsync()
        {
            try
            {
                var itemIds = SetItemList.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                if (itemIds.Count > 0)
                {
                    var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteSelectedItem"]);
                    if (confirmed)
                    {
                        await _setItemAppService.DeleteManyItemsAsync(itemIds);
                        await UpdateItemList();
                        IsAllSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
            }
        }
    }
}
