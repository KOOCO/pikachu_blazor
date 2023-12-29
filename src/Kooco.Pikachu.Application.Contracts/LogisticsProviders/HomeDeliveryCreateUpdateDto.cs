using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.LogisticsProviders
{
    public class HomeDeliveryCreateUpdateDto
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string CustomTitle { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int FreeShippingThreshold { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "This Field Must Be 0 Or Greater")]
        public int Freight { get; set; }

        public string? MainIslands { get; set; }

        public List<string>? MainIslandsList { get; set; }

        public string? OuterIslands { get; set; }

        public List<string>? OuterIslandsList { get; set; }

        public HomeDeliveryCreateUpdateDto()
        {
            MainIslandsList = new List<string>();
            OuterIslandsList = new List<string>();
        }
    }
}
