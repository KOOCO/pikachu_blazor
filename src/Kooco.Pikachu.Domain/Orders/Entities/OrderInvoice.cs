using Kooco.Pikachu.Orders.Invoices;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders.Entities;
public sealed class OrderInvoice : FullAuditedEntity<Guid>
{
    /// <summary>
    /// 發票編號
    /// </summary>
    public required string InvoiceNo { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? UnifiedBusinessNo { get; set; }

    /// <summary>
    /// 創建方式
    /// </summary>
    public required InvoiceCreation CreationType { get; set; }

    /// <summary>
    /// 發票類型
    /// </summary>
    public required InvoiceType InvoiceType { get; set; }

    /// <summary>
    /// 發票狀態
    /// </summary>
    public required InvoiceStatus InvoiceStatus { get; set; }

    /// <summary>
    /// 發票金額
    /// </summary>
    public required decimal SubtotalAmount { get; set; }

    /// <summary>
    /// 運費
    /// </summary>
    public required decimal ShippingCost { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public required decimal TaxAmount { get; set; }

    /// <summary>
    /// 課稅類型
    /// </summary>
    public required InvoiceTaxType TaxType { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public required decimal TotalAmount { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public ICollection<OrderInvoiceItem>? OrderInvoiceItems { get; set; }
}