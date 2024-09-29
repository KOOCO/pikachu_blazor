using AutoMapper;
using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.AddOnProducts
{
    public partial class AddOnProduct
    {
        public IReadOnlyList<AddOnProductDto> AddOnProductList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }

        private GetAddOnProductListDto Filters { get; set; }

        private AddOnProductDto SelectedAddOnProduct { get; set; }
        private List<AddOnProductDto> SelectedAddOnProducts { get; set; }
        public AddOnProduct()
        {
            AddOnProductList = [];
            Filters = new();
        }
        protected override async Task OnInitializedAsync()
        {
            await GetAddOnProductsAsync();
            await base.OnInitializedAsync();
        }
        private async Task GetAddOnProductsAsync()
        {
            try
            {
                var result = await AddOnProductAppService.GetListAsync(
                    new GetAddOnProductListDto
                    {
                        MaxResultCount = PageSize,
                        SkipCount = (CurrentPage - 1) * PageSize,
                        Sorting = CurrentSorting,
                    }
                    );

                AddOnProductList = result.Items;
                TotalCount = (int)result.TotalCount;




            }
            catch (Exception ex)
            {

                await HandleErrorAsync(ex);
            }
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AddOnProductDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;

            await GetAddOnProductsAsync();

            await InvokeAsync(StateHasChanged);
        }
        private static bool RowSelectableHandler(RowSelectableEventArgs<AddOnProductDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
        private void OnAddOnProductStatusChanged(Guid id)
        {
            return;
        }
        async void DeleteAddOn(Guid id) {
            await AddOnProductAppService.DeleteAsync(id);
            await _message.Success("AddOnProductDeletedSucessfully");
           await GetAddOnProductsAsync();
            await InvokeAsync(StateHasChanged);
        }
        void CreateNewAddOnProduct()
        {
            NavigationManager.NavigateTo("/new-add-on-products");


        }
        void NavigateToEditPage(Guid id)
        {
            NavigationManager.NavigateTo("/new-add-on-products/"+id);

        }

    }
}
