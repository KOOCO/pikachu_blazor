using Blazorise.DataGrid;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.StoreComments;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Orders;

public partial class OrderShippingDetails
{
    #region Inject
    [Parameter]
    public string id { get; set; }
    [Parameter]
    public string Ids { get; set; }
    private Guid OrderId { get; set; }
    private List<Guid> OrderIds { get; set; } = [];
    private OrderDto Order { get; set; }
    private List<OrderDto> Orders { get; set; } = [];
    public StoreCommentDto StoreComment { get; set; }
    private bool isPrinting = false;
    #endregion

    #region Methods
    protected async override Task OnInitializedAsync()
    {
        try
        {
            List<Guid> orderIds = JsonConvert.DeserializeObject<List<Guid>>(Ids) ?? [];

            foreach (Guid orderId in orderIds)
            {
                OrderIds.Add(orderId);

                OrderDto orderDto = await _orderAppService.GetWithDetailsAsync(orderId);

                Orders.Add(orderDto);
            }

            await base.OnInitializedAsync(); 
        }
        catch(Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            Console.WriteLine(ex.Message);
        }
    }

    private void PrintDiv()
    {
        isPrinting = true;
        StateHasChanged(); // Trigger a UI update
        JSRuntime.InvokeVoidAsync("printJS"); 
    }
    #endregion
}
