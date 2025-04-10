using Kooco.Pikachu.Orders.Invoices;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單發票
/// </summary>
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
    /// 發票類別
    /// </summary>
    public required InvoiceType InvoiceType { get; set; }

    /// <summary>
    /// 開立方式
    /// </summary>
    public required InvoiceIssuanceMethod IssuanceMethod { get; set; }

    /// <summary>
    /// 小計金額
    /// </summary>
    public required decimal SubtotalAmount { get; set; }

    /// <summary>
    /// 運費
    /// </summary>
    public required decimal ShippingCost { get; set; }

    /// <summary>
    /// 稅務種類
    /// </summary>
    public required InvoiceTaxType TaxType { get; set; }

    /// <summary>
    /// 稅務金額
    /// </summary>
    public required decimal TaxAmount { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public required decimal TotalAmount { get; set; }

    /// <summary>
    /// 序列號
    /// </summary>
    public required short SerialNo { get; set; }

    /// <summary>
    /// 關聯編號
    /// </summary>
    public required string RelateNo { get; set; }

    /// <summary>
    /// 是否作廢
    /// </summary>
    public required bool IsVoided { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public ICollection<OrderInvoiceItem>? OrderInvoiceItems { get; set; }
}