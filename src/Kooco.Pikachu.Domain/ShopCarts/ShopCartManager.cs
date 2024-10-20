﻿using Kooco.Pikachu.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.ShopCarts;

public class ShopCartManager(IShopCartRepository shopCartRepository, IItemRepository itemRepository) : DomainService
{
    public async Task<ShopCart> CreateAsync(Guid userId)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));

        var existing = await shopCartRepository.FindByUserIdAsync(userId);
        if (existing != null)
        {
            throw new ShopCartForUserAlreadyExistsException();
        }

        var shopCart = new ShopCart(GuidGenerator.Create(), userId);
        await shopCartRepository.InsertAsync(shopCart);
        return shopCart;
    }

    public async Task<ShopCart> AddCartItem(ShopCart shopCart, CartItem cartItem)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotNull(cartItem, nameof(cartItem));

        await ValidateItemDetailsAsync(cartItem.ItemId, cartItem.ItemDetailId);
        shopCart.AddCartItem(cartItem);
        return shopCart;
    }

    public async Task<ShopCart> AddCartItem(ShopCart shopCart, Guid itemId, int quantity, int unitPrice, Guid itemDetailId)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotDefaultOrNull<Guid>(itemId, nameof(itemId));
        Check.Range(quantity, nameof(quantity), 0, int.MaxValue);
        Check.Range(unitPrice, nameof(unitPrice), 0, int.MaxValue);
        Check.NotDefaultOrNull<Guid>(itemDetailId, nameof(itemDetailId));

        await ValidateItemDetailsAsync(itemId, itemDetailId);
        shopCart.AddCartItem(GuidGenerator.Create(), itemId, quantity, unitPrice, itemDetailId);
        return shopCart;
    }

    public async Task<CartItem> UpdateCartItemAsync(ShopCart shopCart, Guid cartItemId, Guid itemId, int quantity, int unitPrice, Guid itemDetailId)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotDefaultOrNull<Guid>(cartItemId, nameof(cartItemId));
        Check.NotDefaultOrNull<Guid>(itemId, nameof(itemId));
        Check.Range(quantity, nameof(quantity), 0, int.MaxValue);
        Check.Range(unitPrice, nameof(unitPrice), 0, int.MaxValue);

        var cartItem = shopCart.CartItems.FirstOrDefault(x => x.Id == cartItemId)
                            ?? throw new EntityNotFoundException(typeof(CartItem), cartItemId);

        if (itemId != cartItem.ItemId)
        {
            cartItem.ItemId = itemId;
        }

        if (itemDetailId != cartItem.ItemDetailId)
        {
            await ValidateItemDetailsAsync(itemId, itemDetailId);
            cartItem.ItemDetailId = itemDetailId;
        }

        if (quantity != cartItem.Quantity)
        {
            cartItem.ChangeQuantity(quantity);
        }

        if (unitPrice != cartItem.UnitPrice)
        {
            cartItem.ChangeUnitPrice(unitPrice);
        }

        return cartItem;
    }

    public ShopCart RemoveCartItem(ShopCart shopCart, CartItem cartItem)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotNull(cartItem, nameof(cartItem));

        shopCart.RemoveCartItem(cartItem);
        return shopCart;
    }

    public ShopCart RemoveCartItem(ShopCart shopCart, Guid cartItemId)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotDefaultOrNull<Guid>(cartItemId, nameof(cartItemId));

        shopCart.RemoveCartItem(cartItemId);
        return shopCart;
    }

    public async Task ValidateItemDetailsAsync(Guid itemId, Guid itemDetailId)
    {
        var itemDetail = await itemRepository.FindItemDetailAsync(itemDetailId);
        if (itemDetail == null || itemDetail.ItemId != itemId)
        {
            throw new EntityNotFoundException(typeof(ItemDetails), itemDetailId);
        }
    }
}
