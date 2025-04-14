using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using InvoiceType = Kooco.Pikachu.Orders.Invoices.InvoiceType;

namespace Kooco.Pikachu.Tenants.Entities;

/// <summary>
/// 租戶三方
/// </summary>
public class TenantTripartite : FullAuditedEntity<Guid>, IMultiTenant
{
    /// <summary>
    /// 租戶的唯一識別碼
    /// </summary>
    public required Guid? TenantId { get; set; }

    /// <summary>
    /// 指示是否啟用的旗標
    /// </summary>
    public required bool IsEnable { get; set; }

    /// <summary>
    /// 商店代碼
    /// </summary>
    public required string StoreCode { get; set; }

    /// <summary>
    /// 雜湊金鑰
    /// </summary>
    public required string HashKey { get; set; }

    /// <summary>
    /// 雜湊初始化向量
    /// </summary>
    public required string HashIV { get; set; }

    /// <summary>
    /// 顯示在發票上的名稱
    /// </summary>
    public required string DisplayInvoiceName { get; set; }

    /// <summary>
    /// 發票類型
    /// </summary>
    public required InvoiceType InvoiceType { get; set; }

    /// <summary>
    /// 發票發出時的配送狀態
    /// </summary>
    public required DeliveryStatus StatusOnInvoiceIssue { get; set; }

    /// <summary>
    /// 發貨後生成發票的天數
    /// </summary>
    public required int DaysAfterShipmentGenerateInvoice { get; set; }
}