using Blazorise;
using Kooco.Pikachu.ShopCarts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.ShopCarts;
public partial class CartItemsModal
{
    [Parameter] public IShopCartAppService AppService { get; set; }

    private List<CartItemWithDetailsDto> CartItems { get; set; } = [];
    private ShopCartListWithDetailsDto Selected { get; set; } = new();
    private bool Loading { get; set; }
    private bool ItemsLoading { get; set; }

    private Modal ModalRef;

    public async Task Show(ShopCartListWithDetailsDto input)
    {
        Selected = input ?? new();
        CartItems = [];
        await ModalRef.Show();
        await GetItems();
    }

    public async Task Hide()
    {
        Selected = new();
        await ModalRef.Hide();
    }

    static Task OnModalClosing(ModalClosingEventArgs e)
    {
        e.Cancel = e.CloseReason != CloseReason.UserClosing;
        return Task.CompletedTask;
    }

    async Task Save()
    {
        Loading = true;
        StateHasChanged();
        await Task.Delay(2000);
        Loading = false;
        await Hide();
    }

    async Task GetItems()
    {
        try
        {
            ItemsLoading = true;
            StateHasChanged();

            if (Selected.UserId != Guid.Empty)
            {
                CartItems = await AppService.GetCartItemsListAsync(Selected.Id);
            }

            ItemsLoading = false;
        }
        catch (Exception ex)
        {
            ItemsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    void AddQuantity(CartItemWithDetailsDto cartItem)
    {
        cartItem.Quantity++;
    }

    void MinusQuantity(CartItemWithDetailsDto cartItem)
    {
        if (cartItem.Quantity > 0) cartItem.Quantity--;
    }

    void RemoveItem(CartItemWithDetailsDto cartItem)
    {
        CartItems.Remove(cartItem);
    }
}