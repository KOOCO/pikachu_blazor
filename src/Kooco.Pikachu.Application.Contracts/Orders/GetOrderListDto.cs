using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class GetOrderListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public Guid? GroupBuyId { get; set; }
        public List<Guid>? OrderIds { get; set; }=new List<Guid>();
    
    }
}
