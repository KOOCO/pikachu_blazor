using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.LogisticsProviders;

public class TCat711FrozenCreateUpdateDto
{
    [Required(ErrorMessage = "This Field Is Required")]
    [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
    public int Freight { get; set; }
    public bool Payment { get; set; }
}
