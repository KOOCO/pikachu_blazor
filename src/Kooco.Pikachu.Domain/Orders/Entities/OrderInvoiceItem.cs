using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders.Entities;
public sealed class OrderInvoiceItem : FullAuditedEntity<Guid>
{
    /// <summary>
    /// 產品名稱
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// 產品數量
    /// </summary>
    public int ProductQty { get; set; }

    /// <summary>
    /// 單價 (未稅)
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 總價 (未稅)
    /// </summary>
    public decimal TotalPrice { get; set; }

    public Guid OrderInvoiceId { get; set; }
    public OrderInvoice? OrderInvoice { get; set; }
}