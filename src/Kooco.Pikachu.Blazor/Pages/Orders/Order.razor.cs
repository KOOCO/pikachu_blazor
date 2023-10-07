using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        HashSet<Guid> ExpandedRows = new HashSet<Guid>();

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
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = (bool)e.Value;
            Orders.ForEach(item =>
            {
                item.IsSelected = IsAllSelected;
            });
            StateHasChanged();
        }

        public void OnEditItem(DataGridRowMouseEventArgs<OrderDto> e)
        {
            return;
            var id = e.Item.Id;
            NavigationManager.NavigateTo($"Items/Edit/{id}");
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
    }
}
