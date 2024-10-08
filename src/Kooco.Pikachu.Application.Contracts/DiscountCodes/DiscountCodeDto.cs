﻿using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountCodeDto:AuditedEntityDto<Guid>
    {
        
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Code { get; set; } // Renamed to avoid conflict with class name
        public string SpecifiedCode { get; set; }
        public int AvailableQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public int MaxUsePerPerson { get; set; }
        public string GroupBuysScope { get; set; }
        public string ProductsScope { get; set; }
        public string DiscountMethod { get; set; }
        public int MinimumSpendAmount { get; set; }
        public string ShippingDiscountScope { get; set; }
        public int DiscountPercentage { get; set; }
        public int DiscountAmount { get; set; }
        public bool Status { get; set; }
        public List<DiscountCodeSpecificGroupbuyDto> DiscountSpecificGroupbuys { get; set; } = new();
        public List<DiscountCodeSpecificProductDto> DiscountSpecificProducts { get; set; } = new();
        public int[] SpecificShippingMethods { get; set; }

    }
}
