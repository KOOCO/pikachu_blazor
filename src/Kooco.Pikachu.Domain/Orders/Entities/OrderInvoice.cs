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
    public string InvoiceNo { get; set; }

    /// <summary>
    /// 發票類型
    /// </summary>
    public InvoiceType InvoiceType { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string UnifiedBusinessNo { get; set; }

    /// <summary>
    /// 發票狀態
    /// </summary>
    public InvoiceStatus InvoiceStatus { get; set; }

    /// <summary>
    /// 發票金額
    /// </summary>
    public decimal SubtotalAmount { get; set; }

    /// <summary>
    /// 運費
    /// </summary>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 課稅類型
    /// </summary>
    public InvoiceTaxType TaxType { get; set; }

    /// <summary>
    /// 發票總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 發票創建方式
    /// </summary>
    public InvoiceCreation CreationType { get; set; }

    /// <summary>
    /// 作廢原因
    /// </summary>
    public string VoidReason { get; set; }

    /// <summary>
    /// 作廢時間
    /// </summary>
    public DateTime VoidedTime { get; set; }

    /// <summary>
    /// 開立時間
    /// </summary>
    public DateTime IssueTime { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public ICollection<OrderInvoiceItem>? OrderInvoiceItems { get; set; }
}