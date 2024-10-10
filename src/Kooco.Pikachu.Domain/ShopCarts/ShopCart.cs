using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ShopCarts;

public class ShopCart(Guid id, Guid userId) : FullAuditedAggregateRoot<Guid>(id), IMultiTenant
{
    public Guid UserId { get; set; } = userId;
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();


    public ShopCart AddCartItem(Guid id, Guid itemId, int quantity, int unitPrice, List<string> itemSkus)
    {
        var cartItem = new CartItem(id, Id, itemId, quantity, unitPrice, itemSkus);
        CartItems ??= new List<CartItem>();
        ValidateExistingCartItem(cartItem.ItemId);
        CartItems.Add(cartItem);
        return this;
    }

    public ShopCart AddCartItem(CartItem cartItem)
    {
        Check.NotNull(cartItem, nameof(cartItem));
        CartItems ??= new List<CartItem>();
        ValidateExistingCartItem(cartItem.ItemId);
        CartItems.Add(cartItem);
        return this;
    }

    public ShopCart AddCartItems(List<CartItem> cartItems)
    {
        Check.NotNull(cartItems, nameof(cartItems));
        CartItems ??= new List<CartItem>();
        foreach (var cartItem in cartItems)
        {
            ValidateExistingCartItem(cartItem.ItemId);
            CartItems.Add(cartItem);
        }
        return this;
    }

    public ShopCart SetCartItems(List<CartItem> cartItems)
    {
        Check.NotNull(cartItems, nameof(cartItems));
        CartItems = Check.NotNull(cartItems, nameof(cartItems));
        return this;
    }

    public ShopCart RemoveCartItem(Guid cartItemId)
    {
        Check.NotDefaultOrNull<Guid>(cartItemId, nameof(cartItemId));
        var cartItem = CartItems.FirstOrDefault(x => x.Id == cartItemId)
            ?? throw new EntityNotFoundException(nameof(CartItem));
        CartItems?.Remove(cartItem);
        return this;
    }

    public ShopCart RemoveCartItem(CartItem cartItem)
    {
        Check.NotNull(cartItem, nameof(cartItem));
        CartItems?.Remove(cartItem);
        return this;
    }

    public void ValidateExistingCartItem(Guid itemId)
    {
        var existing = CartItems.FirstOrDefault(x => x.ItemId == itemId);
        if (existing != null)
        {
            throw new CartItemForUserAlreadyExistsException();
        }
    }
}
