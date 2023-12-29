using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.LogisticsProviders
{
    public class GreenWorldLogisticsCreateUpdateDto
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string StoreCode { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string HashKey { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string HashIV { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string SenderPhoneNumber { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string LogisticsType { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string LogisticsSubTypes { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public List<string> LogisticsSubTypesList { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int FreeShippingThreshold { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int Freight { get; set; }

        public GreenWorldLogisticsCreateUpdateDto()
        {
            LogisticsSubTypesList = new();
        }
    }
}
