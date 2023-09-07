using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Items;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Components.Messages;
using Blazorise.DataGrid;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Components;

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
            //_navigationManger.NavigateTo($"Items/Edit/{id}");
        }
    }
}
