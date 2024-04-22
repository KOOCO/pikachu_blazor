using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public class GenerateInvoiceBackgroundJobArgs
    {
        public Guid OrderId { get; set; }
    }
}
