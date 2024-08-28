using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.LogisticsProviders;

public class TCatFrozenCreateUpdateDto
{
    [Required(ErrorMessage = "This Field Is Required")]
    [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
    public int Freight { get; set; }
    [Required(ErrorMessage = "This Field Is Required")]
    [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
    public int OuterIslandFreight { get; set; }
    [Required(ErrorMessage = "This Field Is Required")]
    public SizeEnum Size { get; set; }

    public bool Payment { get; set; }
    public TCatPaymentMethod TCatPaymentMethod { get; set; }
}
