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
            catch
            {
            }
        }
        async Task GetOrderDetailsAsync()
        {
            Order = await _orderAppService.GetWithDetailsAsync(OrderId);
           
            await InvokeAsync(StateHasChanged);
        }
        private void  PrintDiv()
        {
            isPrinting = true;
            StateHasChanged(); // Trigger a UI update

            // This setTimeout ensures that the print dialog is shown after the styles are applied.
            JSRuntime.InvokeVoidAsync("printJS"); // Replace "printJS" with the name of your JavaScript function

        }

    }
}
