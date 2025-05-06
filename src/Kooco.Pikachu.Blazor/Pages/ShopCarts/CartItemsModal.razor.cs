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
    [Parameter] public EventCallback<bool> OnSubmit { get; set; }
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
        try
        {
            if (CartItems.Any(ci => ci.ItemType == ItemType.Item && !ci.ItemDetailId.HasValue))
            {
                foreach (var ci in CartItems.Where(ci => ci.ItemType == ItemType.Item && !ci.ItemDetailId.HasValue))
                {
                    ci.IsInvalid = true;
                }
                return;
            }

            Loading = true;
            StateHasChanged();

            await ShopCartAppService.UpdateShopCartAsync(Selected.Id, CartItems);
            await OnSubmit.InvokeAsync(true);
            Loading = false;
            await Hide();
        }
        catch (Exception ex)
        {
            Loading = false;
            await HandleErrorAsync(ex);
        }
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
        if (cartItem.Quantity < cartItem.Stock)
            cartItem.Quantity++;
    }

    void MinusQuantity(CartItemWithDetailsDto cartItem)
    {
        if (cartItem.Quantity > 0)
            cartItem.Quantity--;
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
                Stock = data.Stock ?? 0,
                Quantity = data.Stock > 0 ? 1 : 0,
                UnitPrice = data.UnitPrice,
                Details = data.Details,
            });

            SelectedItem = null;
        }
    }

    void OnSelectedDetailChanged(CartItemWithDetailsDto item, Guid? detailId)
    {
        if (detailId.HasValue)
        {
            var detail = item.Details.FirstOrDefault(detail => detail.Id == detailId);
            item.UnitPrice = (int?)detail?.UnitPrice ?? 0;
            item.Stock = detail?.Stock ?? 0;
            item.IsInvalid = false;
        }
    }
}