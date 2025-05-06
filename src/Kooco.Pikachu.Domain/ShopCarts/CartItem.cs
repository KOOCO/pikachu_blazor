using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ShopCarts;

public class CartItem : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid ShopCartId { get; private set; }
    public Guid? ItemId { get; private set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; private set; }
    public Guid? TenantId { get; set; }
    public Guid? ItemDetailId { get; private set; }
    public Guid? SetItemId { get; private set; }

    [ForeignKey(nameof(ShopCartId))]
    public virtual ShopCart? ShopCart { get; set; }

    [ForeignKey(nameof(ItemId))]
    public virtual Item? Item { get; set; }

    [ForeignKey(nameof(SetItemId))]
    public virtual SetItem? SetItem { get; set; }


    private CartItem() { }

    public CartItem(
        Guid id,
        Guid shopCartId,
        int quantity,
        int unitPrice,
        Guid? itemId,
        Guid? itemDetailId,
        Guid? setItemId
        ) : base(id)
    {
        ShopCartId = shopCartId;
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
        SpecifyItemOrSetItem(itemId, itemDetailId, setItemId);
    }

    public CartItem SpecifyItemOrSetItem(Guid? itemId, Guid? itemDetailId, Guid? setItemId)
    {
        if ((!itemId.HasValue && !setItemId.HasValue) || (itemId.HasValue && setItemId.HasValue))
        {
            throw new InvalidCartItemException();
        }

        if (itemId.HasValue && !itemDetailId.HasValue)
        {
            throw new EntityNotFoundException(typeof(ItemDetails), itemDetailId);
        }

        ItemId = itemId;
        ItemDetailId = itemDetailId;
        SetItemId = setItemId;

        return this;
    }

    public CartItem ChangeQuantity(int quantity)
    {
        Quantity += quantity;
        return this;
    }

    public void SetQuantity(int quantity)
    {
        Quantity = Check.Range(quantity, nameof(quantity), 0, int.MaxValue);
    }

    public CartItem ChangeUnitPrice(int unitPrice)
    {
        SetUnitPrice(unitPrice);
        return this;
    }

    private void SetUnitPrice(int unitPrice)
    {
        UnitPrice = Check.Range(unitPrice, nameof(unitPrice), 0, int.MaxValue);
    }
}
