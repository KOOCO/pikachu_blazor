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
    public int GroupBuyPrice { get; private set; }
    public int SellingPrice { get; private set; }
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
        int groupBuyPrice,
        int sellingPrice,
        Guid? itemId,
        Guid? itemDetailId,
        Guid? setItemId
        ) : base(id)
    {
        ShopCartId = shopCartId;
        SetQuantity(quantity);
        SetGroupBuyPrice(groupBuyPrice);
        SetSellingPrice(sellingPrice);
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

    public CartItem ChangeGroupBuyPrice(int groupBuyPrice)
    {
        SetGroupBuyPrice(groupBuyPrice);
        return this;
    }

    private void SetGroupBuyPrice(int groupBuyPrice)
    {
        GroupBuyPrice = Check.Range(groupBuyPrice, nameof(groupBuyPrice), 0, int.MaxValue);
    }

    private void SetSellingPrice(int sellingPrice)
    {
        SellingPrice = Check.Range(sellingPrice, nameof(sellingPrice), 0, int.MaxValue);
    }
}
