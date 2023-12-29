using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Items.Dtos
{
    public class GetInventroyInputDto
    {
        public string FilterText { get; set; }
        public string Sorting { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
    }
}
