using Blazorise.DataGrid;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.StoreComments;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Orders
{
    public partial class OrderShippingDetails
    {
        [Parameter]
        public string id { get; set; }
        private Guid OrderId { get; set; }
        private OrderDto Order { get; set; }
        public StoreCommentDto StoreComment { get; set; }
        private bool isPrinting = false;
        protected async override Task OnInitializedAsync()
        {
            try
            {
                OrderId = Guid.Parse(id);
                await GetOrderDetailsAsync();
                await base.OnInitializedAsync();
              
            }
            catch(Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.Message);
            }
        }
        async Task GetOrderDetailsAsync()
        {
            Order = await _orderAppService.GetWithDetailsAsync(OrderId);
            JSRuntime.InvokeVoidAsync("eval", $"document.title = '{Order.OrderNo.ToString()}'");

            await InvokeAsync(StateHasChanged);
        }
        private void  PrintDiv()
        {
            isPrinting = true;
            StateHasChanged(); // Trigger a UI update
             JSRuntime.InvokeVoidAsync("printJS"); 
        }

    }
}
