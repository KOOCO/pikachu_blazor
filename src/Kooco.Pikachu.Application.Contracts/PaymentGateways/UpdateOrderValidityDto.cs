using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.PaymentGateways
{
    public class UpdateOrderValidityDto
    {
        //OrderVAlidatePeriod

        public int Period { get; set; } // Input value (e.g., 5 for 5 days, 5 hours, etc.)
        [Required]
        public string Unit { get; set; }
    }
}
