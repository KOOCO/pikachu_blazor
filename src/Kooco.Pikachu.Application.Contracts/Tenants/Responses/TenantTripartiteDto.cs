using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Application.Dtos;
using InvoiceType = Kooco.Pikachu.Orders.Invoices.InvoiceType;

namespace Kooco.Pikachu.Tenants.Responses;
public class TenantTripartiteDto : CreationAuditedEntityDto<Guid>
{
    public bool IsEnable { get; set; }
    public string StoreCode { get; set; }
    public string HashKey { get; set; }
    public string HashIV { get; set; }
    public string DisplayInvoiceName { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public DeliveryStatus StatusOnInvoiceIssue { get; set; }
    public int DaysAfterShipmentGenerateInvoice { get; set; }
}