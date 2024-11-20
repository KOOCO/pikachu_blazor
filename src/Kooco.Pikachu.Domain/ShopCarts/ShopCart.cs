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
    public Guid GroupBuyId { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();


    public ShopCart AddCartItem(Guid id, Guid itemId, int quantity, int unitPrice, Guid itemDetailId)
    {
        var cartItem = new CartItem(id, Id, itemId, quantity, unitPrice, itemDetailId);
        CartItems ??= new List<CartItem>();
        ValidateExistingCartItem(cartItem.ItemId, cartItem.ItemDetailId);
        CartItems.Add(cartItem);
        return this;
    }

    public ShopCart AddCartItem(CartItem cartItem)
    {
        Check.NotNull(cartItem, nameof(cartItem));
        CartItems ??= new List<CartItem>();
        ValidateExistingCartItem(cartItem.ItemId, cartItem.ItemDetailId);
        CartItems.Add(cartItem);
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

    public void ValidateExistingCartItem(Guid itemId, Guid itemDetailId)
    {
        var existing = CartItems.FirstOrDefault(x => x.ItemId == itemId && x.ItemDetailId == itemDetailId);
        if (existing != null)
        {
            throw new CartItemForUserAlreadyExistsException();
        }
    }
}
