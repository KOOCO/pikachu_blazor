
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PaymentGateways
{
    public class UpdateChinaTrustDto
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? MerchantId { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? TerminalCode { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? CodeValue { get; set; }
    }
}
