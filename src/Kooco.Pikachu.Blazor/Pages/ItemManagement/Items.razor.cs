
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using System.Linq;
using Microsoft.JSInterop;
using Blazorise;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class Items
    {
        private readonly IItemAppService _itemAppService;
        public List<ItemDto> itemList;
        public IEnumerable<ItemDto> selectedRows;
        int _pageIndex = 1;
        int _pageSize = 10;
        int _total = 0;
        IMessageService _message;
        public Items(IItemAppService itemAppService, IMessageService messageService)
        {
            _itemAppService = itemAppService;
            itemList = new List<ItemDto>();
            selectedRows = new List<ItemDto>();
            _message = messageService;
        }
        protected override async Task OnInitializedAsync()
        {
            await UpdateItemList();
        }

        private async Task UpdateItemList()
        {
            int skipCount = (_pageIndex - 1) * _pageSize;
            var result = await _itemAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = _pageSize,
                SkipCount = skipCount
            });
            itemList = result.Items.ToList();
            _total = (int)result.TotalCount;
        }


        //public async Task OnChange(QueryModel<ItemDto> queryModel)
        //{

        //}

        public async Task OnItemAvaliablityChange(Guid id)
        {
            await _itemAppService.ChangeItemAvailability(id);
        }

        public void RemoveSelection(Guid id)
        {
            var selected = selectedRows.Where(x => x.Id != id);
            selectedRows = selected;
        }

        private async void DeleteSelected()
        {
            if (selectedRows.Any())
            {
                await _itemAppService.DeleteManyItems(selectedRows.Select(x => x.Id).ToList());
                await UpdateItemList();
                await _message.Success("successfully deleted selected items");
                StateHasChanged();
            }
            else
                await _message.Warning("No item selected");
        }
        public void CreateNewItem()
        {
            NavigationManager.NavigateTo("Items/New");
        }
    }
}
