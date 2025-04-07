using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PaymentGateways
{
    public class UpdateEcPayDto
    {
        public bool IsCreditCardEnabled { get; set; }
        public bool IsBankTransferEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string MerchantId { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string HashKey { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string HashIV { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string TradeDescription { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string CreditCheckCode { get; set; }

        public List<string> InstallmentPeriods { get; set; } = [];
    }
}
