using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.AddOnProducts
{
    public class GetAddOnProductListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public DateTime? StartDate { get; set;}
        public DateTime? EndDate { get; set;}
    }
}
