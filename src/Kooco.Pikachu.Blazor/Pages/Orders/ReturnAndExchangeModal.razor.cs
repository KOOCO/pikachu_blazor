using Blazorise;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Orders;

public partial class ReturnAndExchangeModal
{
    [Parameter] public OrderDto Order { get; set; }
    [Parameter] public string? PaymentStatus { get; set; }
    private bool IsReturn { get; set; }
    private bool IsWholeOrder { get; set; } = true;
    private Modal Modal { get; set; }

    public async Task Show(bool isReturn)
    {
        IsReturn = isReturn;
        await Modal.Show();
    }

    public async Task Hide()
    {
        await Modal.Hide();
    }

    async Task OnSubmit()
    {
        try
        {
            if (IsReturn)
            {
                await ReturnOrder();
            }
            else
            {
                await ExchangeOrder();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task ExchangeOrder()
    {
        var selectedItems = Order.OrderItems.Where(oi => oi.IsSelected && oi.Quantity > 0).ToList();
        if (!IsWholeOrder && (selectedItems is not { Count: > 0 } || !selectedItems.Any(x => x.SelectedQuantity > 0)))
        {
            await Message.Error(L["PleaseSelectItemsToExchange"]);
            return;
        }

        var confirmed = await Message.Confirm(L["AreYouSureYouWantToExchangeThisOrder"]);
        if (!confirmed)
        {
            return;
        }
        if (IsWholeOrder)
        {
            await ReturnAndExchangeAppService.ExchangeOrderAsync(Order.Id);
        }
        else
        {

            await ReturnAndExchangeAppService.ReturnAndExchangeItemsAsync(Order.Id, selectedItems, IsReturn);
        }

        await Hide();
        NavigationManager.NavigateTo("Orders");
    }

    async Task ReturnOrder()
    {
        var selectedItems = Order.OrderItems.Where(oi => oi.IsSelected && oi.Quantity > 0).ToList();
        if (!IsWholeOrder && (selectedItems is not { Count: > 0 } || !selectedItems.Any(x => x.SelectedQuantity > 0)))
        {
            await Message.Error(L["PleaseSelectItemsToReturn"]);
            return;
        }

        var confirmed = await Message.Confirm(L["Areyousureyouwanttoreturnthisorder"]);
        if (!confirmed)
        {
            return;
        }
        if (IsWholeOrder)
        {
            await ReturnAndExchangeAppService.ReturnOrderAsync(Order.Id);
        }
        else
        {

            await ReturnAndExchangeAppService.ReturnAndExchangeItemsAsync(Order.Id, selectedItems, IsReturn);
        }

        await Hide();
        NavigationManager.NavigateTo("Orders");
        //await JSRuntime.InvokeVoidAsync("reloadOrderPage");
    }
}