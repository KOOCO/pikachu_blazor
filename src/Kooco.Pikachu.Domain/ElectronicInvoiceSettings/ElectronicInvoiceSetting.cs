using Kooco.Pikachu.EnumValues;
using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;
public class ElectronicInvoiceSetting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public bool IsEnable { get; set; }
    public string StoreCode { get; set; }
    public string HashKey { get; set; }
    public string HashIV { get; set; }
    public string DisplayInvoiceName { get; set; }
    public DeliveryStatus StatusOnInvoiceIssue { get; set; }
    public int DaysAfterShipmentGenerateInvoice { get; set; }

    public ElectronicInvoiceSetting()
    {

    }
    public ElectronicInvoiceSetting(
        [NotNull] Guid id,
        bool isEnable,
        string storeCode,
        string hashKey,
        string hashIV,
        string displayInvoiceName,
        DeliveryStatus statusOnInvoiceIssue,
        int dayAfterInvoiceGenerate) : base(id)
    {
        IsEnable = isEnable;
        StoreCode = storeCode;
        HashKey = hashKey;
        HashIV = hashIV;
        DisplayInvoiceName = displayInvoiceName;
        DaysAfterShipmentGenerateInvoice = dayAfterInvoiceGenerate;
        StatusOnInvoiceIssue = statusOnInvoiceIssue;


    }
}
