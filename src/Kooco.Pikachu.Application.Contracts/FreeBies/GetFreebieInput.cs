using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.FreeBies
{
    public class GetFreebieInput: PagedAndSortedResultRequestDto
    {

        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public bool ApplyToAllGroupBuy { get; set; }
        
        public bool UnCondition { get; set; }
        public DateTime? ActivityStartDate { get; set; }
        public DateTime? ActivityEndDate { get; set; }
        public decimal MinimumAmount { get; set; }
        public int MinimumPiece { get; set; }
        public decimal FreebieAmount { get; set; }
       
    }
}
