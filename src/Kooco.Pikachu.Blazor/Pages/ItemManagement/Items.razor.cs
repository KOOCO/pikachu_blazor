using Blazorise;
using Blazorise.Components;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class Items(
        IItemAppService itemAppService,
        IUiMessageService messageService
            )
    {
        public GetItemListDto Filters { get; set; } = new();
        public List<ItemListDto> ItemList { get; set; } = [];
        public List<KeyValueDto> ItemLookup { get; set; } = [];
        LoadingIndicator Loading { get; set; }
        Autocomplete<KeyValueDto, Guid?> Autocomplete { get; set; }

        public bool IsAllSelected = false;
        int PageIndex = 1;
        int Total = 0;
        private readonly IUiMessageService _uiMessageService = messageService;
        private readonly IItemAppService _itemAppService = itemAppService;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ItemLookup = await _itemAppService.GetAllItemsLookupAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ItemListDto> e)
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
                Filters.SkipCount = PageIndex * Filters.MaxResultCount;
                var result = await _itemAppService.GetItemsListAsync(Filters);
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

        public async Task FilterAsync()
        {
            Filters.SkipCount = 0;
            await UpdateItemList();
        }

        public async Task ResetAsync()
        {
            Filters = new();
            await Autocomplete?.Clear();
            await UpdateItemList();
        }

        public async Task OnPageSizeChanged(int value)
        {
            Filters.MaxResultCount = value;
            await UpdateItemList();
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
            IsAllSelected = (bool?)e?.Value ?? false;
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
        public void OnEditItem(DataGridRowMouseEventArgs<ItemListDto> e)
        {
            var id = e.Item.Id;
            NavigationManager.NavigateTo($"Items/Edit/{id}");
        }

        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            Filters.Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateItemList();
        }
    }
}
