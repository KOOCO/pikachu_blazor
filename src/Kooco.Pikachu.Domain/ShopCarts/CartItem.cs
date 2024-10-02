using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.ShopCarts;

public class CartItem : FullAuditedEntity<Guid>
{
    public Guid ShopCartId { get; private set; }
    public Guid ItemId { get; set; }
    public int Quantity { get; private set; }
    public int UnitPrice { get; private set; }

    [ForeignKey(nameof(ShopCartId))]
    public ShopCart? ShopCart { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    public CartItem(Guid id, Guid shopCartId, Guid itemId, int quantity, int unitPrice) : base(id)
    {
        ShopCartId = shopCartId;
        ItemId = itemId;
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
    }

    internal CartItem ChangeQuantity(int quantity)
    {
        SetQuantity(quantity);
        return this;
    }

    private void SetQuantity(int quantity)
    {
        Quantity = Check.Range(quantity, nameof(quantity), 0, int.MaxValue);
    }

    internal CartItem ChangeUnitPrice(int unitPrice)
    {
        SetUnitPrice(unitPrice);
        return this;
    }

    private void SetUnitPrice(int unitPrice)
    {
        UnitPrice = Check.Range(unitPrice, nameof(unitPrice), 0, int.MaxValue);
    }
}
