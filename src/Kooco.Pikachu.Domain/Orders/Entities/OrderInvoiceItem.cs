using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單發票品項
/// </summary>
public sealed class OrderInvoiceItem : FullAuditedEntity<Guid>
{
    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// 商品數量
    /// </summary>
    public int ProductQty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 總價
    /// </summary>
    public decimal TotalPrice { get; set; }

    public Guid OrderInvoiceId { get; set; }
    public OrderInvoice? OrderInvoice { get; set; }
}