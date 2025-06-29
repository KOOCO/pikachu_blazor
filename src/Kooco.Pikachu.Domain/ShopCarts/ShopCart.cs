﻿using System;
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
    public virtual IdentityUser? User { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();


    public ShopCart AddCartItem(Guid id, int quantity, int groupBuyPrice,int sellingPrice ,Guid? itemId, Guid? itemDetailId, Guid? setItemId)
    {
        var cartItem = new CartItem(id, Id, quantity, groupBuyPrice,sellingPrice, itemId, itemDetailId, setItemId);
        CartItems ??= new List<CartItem>();
        ValidateExistingCartItem(cartItem.ItemId, cartItem.ItemDetailId, cartItem.SetItemId);
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

    public void ValidateExistingCartItem(Guid? itemId, Guid? itemDetailId, Guid? setItemId)
    {
        var existing = CartItems.FirstOrDefault(x => (setItemId.HasValue && x.SetItemId == setItemId)
                            || (itemId.HasValue && x.ItemId == itemId && x.ItemDetailId == itemDetailId));
        if (existing != null)
        {
            throw new CartItemForUserAlreadyExistsException();
        }
    }
}
