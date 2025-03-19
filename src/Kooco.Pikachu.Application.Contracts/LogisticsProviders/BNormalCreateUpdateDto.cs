using Kooco.Pikachu.EnumValues;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.LogisticsProviders
{
    public class BNormalCreateUpdateDto
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int Freight { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int OuterIslandFreight { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public SizeEnum Size { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int HualienAndTaitungShippingFee { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int HolidaySurcharge { get; set; }

        public DateTime? HolidaySurchargeStartTime { get; set; }

        public DateTime? HolidaySurchargeEndTime { get; set; }
    }
}
