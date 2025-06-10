using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Orders;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryLogDto : FullAuditedEntityDto<Guid>, IMultiTenant
{
    public Guid ItemId { get; set; }
    public Guid ItemDetailId { get; set; }
    public string? Sku { get; set; }
    public string? Attributes { get; set; }
    public InventoryStockType StockType { get; set; }
    public InventoryActionType ActionType { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? TenantId { get; set; }
    public virtual ItemDto Item { get; set; }
    public virtual ItemDetailsDto ItemDetail { get; set; }
    public virtual OrderDto Order { get; set; }
}
