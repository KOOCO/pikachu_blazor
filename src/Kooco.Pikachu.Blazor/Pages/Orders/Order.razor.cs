using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.Blazor.Pages.ItemManagement;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Kooco.Pikachu.Blazor.Pages.Orders
{
    public partial class Order
    {
        private bool IsAllSelected { get; set; } = false;
        private List<OrderDto> Orders { get; set; } = new();
        private int TotalCount { get; set; }
        private OrderDto SelectedOrder { get; set; }
        private int PageIndex { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private string? Sorting { get; set; }
        private string? Filter { get; set; }

        private readonly HashSet<Guid> ExpandedRows = new();
        private LoadingIndicator loading { get; set; }
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
        {
            PageIndex = e.Page - 1;
            await UpdateItemList();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateItemList()
        {
            try
            {
                await loading.Show();
                int skipCount = PageIndex * PageSize;
                var result = await _orderAppService.GetListAsync(new GetOrderListDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount,
                    Filter = Filter
                });
                Orders = result?.Items.ToList() ?? new List<OrderDto>();
                TotalCount = (int?)result?.TotalCount ?? 0;

                await loading.Hide();
            }
            catch (Exception ex)
            {
                await loading.Hide();
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        async Task OnSearch()
        {
            PageIndex = 0;
            await UpdateItemList();
        }

        void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = e.Value != null ? (bool)e.Value : false;
            Orders.ForEach(item =>
            {
                item.IsSelected = IsAllSelected;
            });
            StateHasChanged();
        }

        public async void NavigateToOrderDetails(DataGridRowMouseEventArgs<OrderDto> e)
        {
            await loading.Show();

            var id = e.Item.Id;
            NavigationManager.NavigateTo($"Orders/OrderDetails/{id}");

            await loading.Hide();
        }

        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateItemList();
        }

        void ToggleRow(DataGridRowMouseEventArgs<OrderDto> e)
        {
            if (ExpandedRows.Contains(e.Item.Id))
            {
                ExpandedRows.Remove(e.Item.Id);
            }
            else
            {
                ExpandedRows.Add(e.Item.Id);
            }
        }
        public void NavigateToOrderPrint()
        {
            var selectedOrder = Orders.SingleOrDefault(x => x.IsSelected);
            NavigationManager.NavigateTo($"Orders/OrderShippingDetails/{selectedOrder.Id}");
        }

    }
}
