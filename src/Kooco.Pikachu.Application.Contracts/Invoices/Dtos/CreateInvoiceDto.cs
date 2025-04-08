using Kooco.Pikachu.Orders.Invoices;
using System;

namespace Kooco.Pikachu.Invoices.Dtos;
public class CreateInvoiceDto
{
    public Guid OrderId { get; set; }
    public string InvoiceNo { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public string UnifiedBusinessNo { get; set; }
    public InvoiceStatus InvoiceStatus { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public InvoiceTaxType TaxType { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceCreation CreationType { get; set; }
    public string VoidReason { get; set; }
    public DateTime IssueTime { get; set; }
}