using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.ShopCarts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.ShopCarts;
public partial class CartItemsModal
{
    [Parameter] public IShopCartAppService AppService { get; set; }

    private List<CartItemWithDetailsDto> CartItems { get; set; } = [];
    private ShopCartListWithDetailsDto Selected { get; set; } = new();
    private List<ItemWithItemTypeDto> ProductOptions { get; set; } = [];
    private ItemWithItemTypeDto? SelectedItem { get; set; } = null;

    private bool Loading { get; set; }
    private bool ItemsLoading { get; set; }

    private Modal ModalRef;

    public async Task Show(ShopCartListWithDetailsDto input)
    {
        Selected = input ?? new();
        CartItems = [];
        await ModalRef.Show();
        await GetItems();
        await GetProductsLookup();
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
        if (CartItems.Count == 0)
        {
            return;
        }

        if (CartItems.Any(ci => !ci.ItemDetailId.HasValue))
        {
            foreach (var ci in CartItems.Where(ci => !ci.ItemDetailId.HasValue))
            {
                ci.IsInvalid = true;
            }
            return;
        }

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

    async Task GetProductsLookup()
    {
        try
        {
            SelectedItem = null;
            ProductOptions.Clear();
            ProductOptions = await ShopCartAppService.GetGroupBuyProductsLookupAsync(Selected.GroupBuyId);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task OnSelectedItemChanged(ItemWithItemTypeDto item)
    {
        if (item != null)
        {
            var data = await ShopCartAppService.GetItemWithDetailsAsync(Selected.GroupBuyId, item.Id, item.ItemType);
            CartItems.Add(new CartItemWithDetailsDto
            {
                Id = null,
                ShopCartId = Selected.Id,
                ItemType = item.ItemType,
                ItemId = data.ItemType == ItemType.Item ? data.Id : null,
                SetItemId = data.ItemType == ItemType.SetItem ? data.Id : null,
                ItemName = item.Name,
                Image = data.Image,
                Quantity = 1,
                UnitPrice = data.UnitPrice,
                Details = data.Details
            });

            SelectedItem = null;
        }
    }

    void OnSelectedDetailChanged(CartItemWithDetailsDto item, Guid? detailId)
    {
        if (detailId.HasValue)
        {
            var detail = item.Details.FirstOrDefault(detail => detail.Id == detailId);
            item.UnitPrice = detail?.UnitPrice ?? 0;
            item.IsInvalid = false;
        }
    }
}