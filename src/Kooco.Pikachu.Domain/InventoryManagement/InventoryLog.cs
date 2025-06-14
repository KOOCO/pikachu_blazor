using Kooco.Pikachu.Items;
using Kooco.Pikachu.Orders.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryLog : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid ItemId { get; private set; }
    public Guid ItemDetailId { get; private set; }

    [MaxLength(InventoryLogConsts.MaxSkuLength)]
    public string? Sku { get; private set; }

    [MaxLength(InventoryLogConsts.MaxAttributesLength)]
    public string? Attributes { get; private set; }
    public InventoryActionType ActionType { get; set; }
    public int StockOnHand { get; private set; }
    public int SaleableQuantity { get; private set; }
    public int PreOrderQuantity { get; private set; }
    public int SaleablePreOrderQuantity { get; private set; }

    [MaxLength(InventoryLogConsts.MaxDescriptionLength)]
    public string? Description { get; private set; }
    public Guid? OrderId { get; private set; }
    public string? OrderNumber { get; private set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(ItemId))]
    public virtual Item Item { get; set; }

    [ForeignKey(nameof(ItemDetailId))]
    public virtual ItemDetails ItemDetail { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; }

    public InventoryLog(
        Guid id,
        Guid itemId,
        Guid itemDetailId,
        string sku,
        string attributes,
        InventoryActionType actionType,
        int stockOnHand,
        int saleableQuantity,
        int preOrderQuantity,
        int saleablePreOrderQuantity,
        string? description,
        Guid? orderId = null,
        string? orderNumber = null
        ) : base(id)
    {
        SetItem(itemId, itemDetailId);
        SetSku(sku);
        SetAttributes(attributes);
        SetActionType(actionType);
        SetAmount(stockOnHand, saleableQuantity, preOrderQuantity, saleablePreOrderQuantity);
        SetDescription(description);
        SetOrder(orderId, orderNumber);
    }

    public InventoryLog SetItem(Guid itemId, Guid itemDetailId)
    {
        ItemId = Check.NotDefaultOrNull<Guid>(itemId, nameof(ItemId));
        ItemDetailId = Check.NotDefaultOrNull<Guid>(itemDetailId, nameof(ItemDetailId));
        return this;
    }

    public InventoryLog SetSku(string? sku)
    {
        Sku = Check.NotNullOrWhiteSpace(
            sku,
            nameof(Sku),
            maxLength: InventoryLogConsts.MaxSkuLength
            );

        return this;
    }

    public InventoryLog SetAttributes(string? attributes)
    {
        Attributes = Check.NotNullOrWhiteSpace(
            attributes,
            nameof(Attributes),
            maxLength: InventoryLogConsts.MaxAttributesLength
            );

        return this;
    }

    public InventoryLog SetActionType(InventoryActionType actionType)
    {
        if (!Enum.IsDefined(actionType))
        {
            throw new ArgumentOutOfRangeException(nameof(actionType), "Invalid action type provided.");
        }
        ActionType = Check.NotNull(actionType, nameof(actionType));
        return this;
    }

    public InventoryLog SetAmount(int stockOnHand, int saleableQuantity, int preOrderQuantity, int saleablePreOrderQuantity)
    {
        StockOnHand = stockOnHand;
        SaleableQuantity = saleableQuantity;
        PreOrderQuantity = preOrderQuantity;
        SaleablePreOrderQuantity = saleablePreOrderQuantity;
        return this;
    }

    public InventoryLog SetDescription(string? description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(Description),
            maxLength: InventoryLogConsts.MaxDescriptionLength
            );

        return this;
    }

    public InventoryLog SetOrder(Guid? orderId, string? orderNumber)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        return this;
    }
}
