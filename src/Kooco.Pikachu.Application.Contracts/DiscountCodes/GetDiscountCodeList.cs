using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.DiscountCodes
{
    public class GetDiscountCodeList : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
