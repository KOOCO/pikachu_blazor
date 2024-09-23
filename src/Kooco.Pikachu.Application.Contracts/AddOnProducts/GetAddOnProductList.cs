using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.AddOnProducts
{
    public class GetAddOnProductList : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public DateTime? From { get; set;}
        public DateTime? To { get; set;}
    }
}
