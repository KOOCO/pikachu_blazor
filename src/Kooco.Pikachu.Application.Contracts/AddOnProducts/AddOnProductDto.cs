using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.AddOnProducts
{
    public class AddOnProductDto:AuditedEntityDto<int>
    {
        public int ProductId { get; set; }
        public int AddOnAmount { get; set; }
        public int SellingQuantity { get; set; }
        public string ProductName { get; set; }
        public int AddOnLimitPerOrder { get; set; }
        public string QuantitySetting { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AddOnConditions { get; set; }
        public int MinimumSpendAmount { get; set; }
        public string GroupBuysScope { get; set; }
        public bool Status { get; set; }
    }
}
