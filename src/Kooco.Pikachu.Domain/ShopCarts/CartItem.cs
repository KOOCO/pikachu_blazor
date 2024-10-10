using Kooco.Pikachu.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ShopCarts;

public class CartItem : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid ShopCartId { get; private set; }
    public Guid ItemId { get; set; }
    public int Quantity { get; private set; }
    public int UnitPrice { get; private set; }
    public Guid? TenantId { get; set; }
    public string ItemSkuJson { get; private set; }

    [ForeignKey(nameof(ShopCartId))]
    public ShopCart? ShopCart { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Item? Item { get; set; }

    [NotMapped]
    public List<string> ItemSkus
    {
        get
        {
            try { return JsonSerializer.Deserialize<List<string>>(ItemSkuJson) ?? []; }
            catch { return []; }
        }
    }

    private CartItem() { }

    public CartItem(Guid id, Guid shopCartId, Guid itemId, int quantity, int unitPrice, List<string> itemSkus) : base(id)
    {
        ShopCartId = shopCartId;
        ItemId = itemId;
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
        SetItemSkus(itemSkus);
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

    internal CartItem SetItemSkus(List<string> skus)
    {
        ItemSkuJson = JsonSerializer.Serialize(skus);
        return this;
    }
}
