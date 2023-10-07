using Blazorise.DataGrid;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Orders
{
    public partial class Order
    {
        private bool IsAllSelected { get; set; } = false;
        private List<OrderDto> Orders { get; set; } = new();
        private long TotalCount { get; set; }
        private OrderDto SelectedOrder { get; set; }
        private int PageIndex { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private string? Sorting { get; set; }
        private string? Filter { get; set; }


        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
        {
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
                TotalCount = result?.TotalCount ?? 0;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
            }
        }


        async void HandleSelectAllChange(ChangeEventArgs e)
        {
            await _uiMessageService.Confirm("Hello World");
            //IsAllSelected = (bool)e.Value;
            //ItemList.ForEach(item =>
            //{
            //    item.IsSelected = IsAllSelected;
            //});
            //StateHasChanged();
        }
    }
}
