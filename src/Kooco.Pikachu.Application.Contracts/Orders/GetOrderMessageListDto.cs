using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class GetOrderMessageListDto:PagedAndSortedResultRequestDto
    {
        public Guid? OrderId { get; set; }

        public Guid? SenderId { get; set; }

        public string? Filter { get; set; }
        public string? OrderNo { get; set; }

        public DateTime? Timestamp { get; set; }

        
        public bool? IsMerchant { get; set; }

    }
}
