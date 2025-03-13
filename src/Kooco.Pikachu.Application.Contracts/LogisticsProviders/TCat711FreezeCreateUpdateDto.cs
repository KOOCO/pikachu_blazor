using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.LogisticsProviders;

public class TCat711FreezeCreateUpdateDto
{
    public bool IsEnabled { get; set; }

    [Required(ErrorMessage = "This Field Is Required")]
    [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
    public int Freight { get; set; }
    public bool Payment { get; set; }
}
