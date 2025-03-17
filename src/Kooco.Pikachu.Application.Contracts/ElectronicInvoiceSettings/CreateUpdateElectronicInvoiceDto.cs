using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
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
        public int? DaysAfterShipmentGenerateInvoice { get; set; }
        
        [Required]
        public DeliveryStatus? StatusOnInvoiceIssue { get; set; }
    }
}
