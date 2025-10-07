using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.ReturnAndExchange;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Orders;

public partial class ReturnAndExchangeModal
{
    [Parameter] public OrderDto Order { get; set; }
    [Parameter] public string? PaymentStatus { get; set; }
    private bool IsReturn { get; set; }
    private bool IsWholeOrder { get; set; } = true;
    private bool ChooseReplacement { get; set; }
    private Modal Modal { get; set; }
    private List<ItemWithItemTypeDto> ItemOptions { get; set; } = [];
    private Dictionary<Guid, List<ItemDetailWithItemTypeDto>> ItemDetails { get; set; } = [];
    private List<ReplacementItemDto> ReplacementItems { get; set; } = [];

    public async Task Show(bool isReturn)
    {
        IsReturn = isReturn;
        ChooseReplacement = false;
        Order.OrderItems.ForEach(oi => oi.IsSelected = false);

        if (isReturn)
        {
            ItemDetails = [];
        }
        else
        {
            ItemOptions = await ReturnAndExchangeAppService.GetGroupBuyItemsAsync(Order.GroupBuyId);
        }
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
        if (!IsWholeOrder)
        {
            if (selectedItems is not { Count: > 0 } || !selectedItems.Any(x => x.SelectedQuantity > 0))
            {
                await Message.Error(L["PleaseSelectItemsToExchange"]);
                return;
            }
            if (ReplacementItems is not { Count: > 0 } || !ReplacementItems.TrueForAll(x => x.ReplacementQuantity > 0))
            {
                await Message.Error(L["PleaseSelectReplacementItems"]);
                return;
            }
            if (ReplacementItems.Any(ri => ri.Item == null || (ri.Item.ItemType == ItemType.Item && ri.Detail == null)))
            {
                await Message.Error(L["PleaseSelectReplacementItems"]);
                return;
            }
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
            await ReturnAndExchangeAppService.ReturnAndExchangeItemsAsync(Order.Id, selectedItems, IsReturn, ReplacementItems);
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
            await ReturnAndExchangeAppService.ReturnAndExchangeItemsAsync(Order.Id, selectedItems, IsReturn, ReplacementItems);
        }

        await Hide();
        NavigationManager.NavigateTo("Orders");
    }

    async Task OnItemChanged(ItemWithItemTypeDto input, ReplacementItemDto item)
    {
        item.Detail = null;
        item.Pricing = null;
        item.ReplacementPrice = 0;

        if (input == null)
        {
            return;
        }

        if (input.ItemType == ItemType.Item && !ItemDetails.ContainsKey(input.Id))
        {
            var details = await ReturnAndExchangeAppService.GetItemDetailsAsync(Order.GroupBuyId, input);
            ItemDetails.TryAdd(input.Id, details);
        }
        else if (input.ItemType == ItemType.SetItem)
        {
            var pricing = await ReturnAndExchangeAppService.GetSetItemPricingAsync(Order.GroupBuyId, input.Id);
            item.Pricing = pricing;
            item.ReplacementPrice = (decimal)(pricing?.Price ?? 0);
        }
    }

    static void OnDetailChanged(ItemDetailWithItemTypeDto input, ReplacementItemDto item)
    {
        item.Pricing = null; 
        item.ReplacementPrice = 0;
        
        if (input == null) return;

        item.Pricing = item.Detail?.Pricing;
        item.ReplacementPrice = (decimal)(item.Pricing?.Price ?? 0);
    }

    static string ItemImage(OrderItemDto item)
    {
        string? firstImage = item.ItemType switch
        {
            ItemType.Item => item.Item?.Images?.FirstOrDefault()?.ImageUrl,
            ItemType.SetItem => item.SetItem?.Images?.FirstOrDefault()?.ImageUrl,
            ItemType.Freebie => item.Freebie?.Images?.FirstOrDefault()?.ImageUrl,
            _ => null
        };

        return firstImage;
    }

    string ItemName(OrderItemDto item)
    {
        string? name = item.ItemType switch
        {
            ItemType.Item => item.Item?.ItemName,
            ItemType.SetItem => item.SetItem?.SetItemName,
            ItemType.Freebie => item.Freebie?.ItemName,
            _ => null
        };

        if (item.IsAddOnProduct)
        {
            name += " " + L["(AddOnProduct)"];
        }

        return name;
    }

    void ReplaceItems()
    {
        ReplacementItems = [.. Order.OrderItems
            .Where(oi => oi.IsSelected && oi.SelectedQuantity > 0 && oi.ItemType != ItemType.Freebie && !oi.IsAddOnProduct)
            .Select(oi => new ReplacementItemDto
            {
                OrderItemId = oi.Id,
                Name = ItemName(oi),
                Image = ItemImage(oi),
                Spec = oi.Spec,
                SelectedQuantity = oi.SelectedQuantity,
                ItemPrice = oi.ItemPrice,
                TotalAmount = oi.TotalAmount
            })];

        ChooseReplacement = true;
        StateHasChanged();
    }

    bool ChooseReplacementDisabled()
    {
        var items = Order.OrderItems.Where(oi => oi.IsSelected && !oi.IsAddOnProduct && oi.ItemType != ItemType.Freebie);
        return !items.Any()
            || !items.ToList().TrueForAll(oi => oi.SelectedQuantity > 0);
    }
}