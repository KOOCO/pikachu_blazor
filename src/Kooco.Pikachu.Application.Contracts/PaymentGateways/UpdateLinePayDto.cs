using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PaymentGateways
{
    public class UpdateLinePayDto
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "Validation:Required")]
        public string ChannelId { get; set; }

        [Required(ErrorMessage = "Validation:Required")]
        public string ChannelSecretKey { get; set; }

        public bool LinePointsRedemption { get; set; }
    }
}
