using AntDesign.TableModels;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using System.Linq;

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
        public Items(IItemAppService itemAppService)
        {
            _itemAppService = itemAppService;
            itemList = new List<ItemDto>();
        }
        protected override async Task OnInitializedAsync()
        {
            int skipCount =  (_pageIndex - 1)  * _pageSize;
            var result = await _itemAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = _pageSize,
                SkipCount = skipCount
            });
            itemList = result.Items.ToList();
            _total = (int)result.TotalCount;
        }

        public async Task OnChange(QueryModel<ItemDto> queryModel)
        {

        }

        public void RemoveSelection(Guid id)
        {
            var selected = selectedRows.Where(x => x.Id != id);
            selectedRows = selected;
        }

        private void Delete(int id)
        {

        }
    }
}
