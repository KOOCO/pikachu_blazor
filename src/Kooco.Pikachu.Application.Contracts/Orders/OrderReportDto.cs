using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class OrderReportDto: PagedAndSortedResultRequestDto
    {
        public Guid? GroupBuyId { get; set; }
        public string? GroupBuyName { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}
