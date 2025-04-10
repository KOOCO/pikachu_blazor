using Kooco.Pikachu.EnumValues;
using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using InvoiceType = Kooco.Pikachu.Orders.Invoices.InvoiceType;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;

/// <summary>
/// 電子發票設定
/// </summary>
public class ElectronicInvoiceSetting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租戶的唯一識別碼
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 指示是否啟用的旗標
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 商店代碼
    /// </summary>
    public string StoreCode { get; set; }

    /// <summary>
    /// 雜湊金鑰
    /// </summary>
    public string HashKey { get; set; }

    /// <summary>
    /// 雜湊初始化向量
    /// </summary>
    public string HashIV { get; set; }

    /// <summary>
    /// 顯示在發票上的名稱
    /// </summary>
    public string DisplayInvoiceName { get; set; }

    /// <summary>
    /// 發票類型
    /// </summary>
    public InvoiceType InvoiceType { get; set; }

    /// <summary>
    /// 發票發出時的配送狀態
    /// </summary>
    public DeliveryStatus StatusOnInvoiceIssue { get; set; }

    /// <summary>
    /// 發貨後生成發票的天數
    /// </summary>
    public int DaysAfterShipmentGenerateInvoice { get; set; }

    public ElectronicInvoiceSetting() { }
    public ElectronicInvoiceSetting(
        [NotNull] Guid id,
        bool isEnable,
        string storeCode,
        string hashKey,
        string hashIV,
        string displayInvoiceName,
        InvoiceType invoiceType,
        DeliveryStatus statusOnInvoiceIssue,
        int dayAfterInvoiceGenerate) : base(id)
    {
        IsEnable = isEnable;
        StoreCode = storeCode;
        HashKey = hashKey;
        HashIV = hashIV;
        DisplayInvoiceName = displayInvoiceName;
        InvoiceType = invoiceType;
        DaysAfterShipmentGenerateInvoice = dayAfterInvoiceGenerate;
        StatusOnInvoiceIssue = statusOnInvoiceIssue;
    }
}