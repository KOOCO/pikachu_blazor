using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.LogisticsProviders
{
    public class PostOfficeCreateUpdateDto
    {
        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int Freight { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, 999, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public decimal Weight { get; set; }

    }
}
