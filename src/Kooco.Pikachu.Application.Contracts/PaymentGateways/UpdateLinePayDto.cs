using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PaymentGateways
{
    public class UpdateLinePayDto
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string ChannelId { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string ChannelSecretKey { get; set; }

        public bool LinePointsRedemption { get; set; }
    }
}
