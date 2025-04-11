using System;

namespace Kooco.Pikachu.ElectronicInvoiceSettings;
public class GenerateInvoiceBackgroundJobArgs
{
    public Guid OrderId { get; set; }
}