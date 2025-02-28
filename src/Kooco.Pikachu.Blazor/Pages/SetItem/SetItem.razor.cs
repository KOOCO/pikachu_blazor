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
using Blazorise;

namespace Kooco.Pikachu.Blazor.Pages.SetItem
{
    public partial class SetItem
    {
        public List<SetItemDto> SetItemList;
        public bool IsAllSelected = false;
        int PageIndex = 1;
        int PageSize = 10;
        int Total = 0;
        private bool loading { get; set; } = true;
        private string Sorting = nameof(SetItemDto.SetItemName);

        private readonly IUiMessageService _uiMessageService;
        private readonly ISetItemAppService _setItemAppService;

        public SetItem(IUiMessageService messageService, ISetItemAppService setItemAppService)
        {
            _uiMessageService = messageService;
            _setItemAppService = setItemAppService;
            SetItemList = new List<SetItemDto>();
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SetItemDto> e)
        {
            loading = true;
            PageIndex = e.Page - 1;
            await UpdateItemList();
            await InvokeAsync(StateHasChanged);
            loading = false;
        }

        private async Task UpdateItemList()
        {
            try
            {
                loading = true;
                int skipCount = PageIndex * PageSize;
                var result = await _setItemAppService.GetListAsync(new PagedAndSortedResultRequestDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount
                });
                SetItemList = result.Items.ToList();
                Total = (int)result.TotalCount;
                loading = false;
            }
            catch (Exception ex)
            {
                loading = false;
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        public void OnEditItem(SetItemDto e)
        {
            var id = e.Id;
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
                        loading = true;
                        await _setItemAppService.DeleteManyItemsAsync(itemIds);
                        await UpdateItemList();
                        IsAllSelected = false;
                        loading = false;
                    }
                }
            }
            catch (Exception ex)
            {
                loading = false;
                await _uiMessageService.Error(ex.GetType()?.ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateItemList();
        }
    }
}
