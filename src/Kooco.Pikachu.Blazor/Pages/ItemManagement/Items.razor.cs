using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using System.Linq;
using Blazorise.DataGrid;
using Volo.Abp;

using Volo.Abp.AspNetCore.Components.Messages;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazorise.LoadingIndicator;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class Items
    {
        public List<ItemDto> ItemList;
        public bool IsAllSelected = false;
        int PageIndex = 1;
        int PageSize = 10;
        int Total = 0;
        string Sorting = nameof(ItemDto.ItemName);

        private readonly IUiMessageService _uiMessageService;
        private readonly IItemAppService _itemAppService;
        LoadingIndicator Loading { get; set; }
        public Items(
            IItemAppService itemAppService, 
            IUiMessageService messageService
            )
        {
            _itemAppService = itemAppService;
            _uiMessageService = messageService;

            ItemList = new List<ItemDto>();
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ItemDto> e)
        {
            PageIndex = e.Page - 1;
            await UpdateItemList();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateItemList()
        {
            try
            {
                await Loading.Show();
                int skipCount = PageIndex * PageSize;
                var result = await _itemAppService.GetListAsync(new PagedAndSortedResultRequestDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount
                });
                ItemList = result.Items.ToList();
                Total = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                await Loading.Hide();
            }
        }

        public async Task OnItemAvaliablityChange(Guid id)
        {
            try
            {
                await Loading.Show();
                var item = ItemList.Where(x => x.Id == id).First();
                item.IsItemAvaliable = !item.IsItemAvaliable;
                await _itemAppService.ChangeItemAvailability(id);
                await UpdateItemList();
                await InvokeAsync(StateHasChanged);
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(ex.Code.ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                await Loading.Hide();
            }
        }
        private void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = (bool)e.Value;
            ItemList.ForEach(item =>
            {
                item.IsSelected = IsAllSelected;
            });
            StateHasChanged();
        }
        private async Task DeleteSelectedAsync()
        {
            try
            {
                var itemIds = ItemList.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                if (itemIds.Count > 0)
                {
                    var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteSelectedItem"]);
                    if (confirmed)
                    {
                        await Loading.Show();
                        await _itemAppService.DeleteManyItemsAsync(itemIds);
                        await UpdateItemList();
                        IsAllSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                await Loading.Hide();
            }
        }
        public void CreateNewItem()
        {
            NavigationManager.NavigateTo("Items/New");
        }
        public void OnEditItem(DataGridRowMouseEventArgs<ItemDto> e)
        {
            var id = e.Item.Id;
            NavigationManager.NavigateTo($"Items/Edit/{id}");
        }

        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateItemList();
        }
    }
}
