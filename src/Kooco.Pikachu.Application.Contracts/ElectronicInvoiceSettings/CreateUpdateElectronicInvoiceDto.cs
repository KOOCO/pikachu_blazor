using Kooco.Pikachu.EnumValues;
using System.ComponentModel.DataAnnotations;
using InvoiceType = Kooco.Pikachu.Orders.Invoices.InvoiceType;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;
public class CreateUpdateElectronicInvoiceDto
{
    public bool IsEnable { get; set; }

    [Required]
    public string StoreCode { get; set; }

    [Required]
    public string HashKey { get; set; }

    [Required]
    public string HashIV { get; set; }

    [Required]
    public string DisplayInvoiceName { get; set; }

    [Required]
    public InvoiceType InvoiceType { get; set; }

    [Required]
    public int? DaysAfterShipmentGenerateInvoice { get; set; }

    [Required]
    public DeliveryStatus? StatusOnInvoiceIssue { get; set; }
}