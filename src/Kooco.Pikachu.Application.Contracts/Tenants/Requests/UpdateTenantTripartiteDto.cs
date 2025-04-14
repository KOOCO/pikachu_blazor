using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;
using InvoiceType = Kooco.Pikachu.Orders.Invoices.InvoiceType;

namespace Kooco.Pikachu.Tenants.Requests;
public class UpdateTenantTripartiteDto : FullAuditedEntityDto<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public required bool IsEnable { get; set; }
    public required string StoreCode { get; set; }
    public required string HashKey { get; set; }
    public required string HashIV { get; set; }
    public required string DisplayInvoiceName { get; set; }
    public required InvoiceType InvoiceType { get; set; }
    public required DeliveryStatus StatusOnInvoiceIssue { get; set; }
    public required int DaysAfterShipmentGenerateInvoice { get; set; }
}