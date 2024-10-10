using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.ShopCarts;

public class ShopCartManager(IShopCartRepository shopCartRepository) : DomainService
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

    public ShopCart AddCartItem(ShopCart shopCart, CartItem cartItem)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotNull(cartItem, nameof(cartItem));

        shopCart.AddCartItem(cartItem);
        return shopCart;
    }

    public ShopCart AddCartItem(ShopCart shopCart, Guid itemId, int quantity, int unitPrice, List<string> itemSkus)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotDefaultOrNull<Guid>(itemId, nameof(itemId));
        Check.Range(quantity, nameof(quantity), 0, int.MaxValue);
        Check.Range(unitPrice, nameof(unitPrice), 0, int.MaxValue);
        Check.NotNull(itemSkus, nameof(itemSkus));

        shopCart.AddCartItem(GuidGenerator.Create(), itemId, quantity, unitPrice, itemSkus);
        return shopCart;
    }

    public ShopCart AddCartItems(ShopCart shopCart, List<CartItem> cartItems)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotNull(cartItems, nameof(cartItems));

        shopCart.AddCartItems(cartItems);
        return shopCart;
    }

    public ShopCart SetCartItems(ShopCart shopCart, List<CartItem> cartItems)
    {
        Check.NotNull(shopCart, nameof(shopCart));
        Check.NotNull(cartItems, nameof(cartItems));

        shopCart.SetCartItems(cartItems);
        return shopCart;
    }

    public CartItem UpdateCartItem(ShopCart shopCart, Guid cartItemId, Guid itemId, int quantity, int unitPrice)
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
}
