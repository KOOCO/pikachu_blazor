using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.FreeBies.Dtos;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Freebies.Dtos
{
    public class FreebieCreateDto
    {
        public string ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public bool ApplyToAllGroupBuy { get; set; } = true;
        public ICollection<CreateImageDto> Images { get; set; } = new List<CreateImageDto>();
        public List<Guid?> FreebieGroupBuys { get; set; } = new List<Guid?>();
        public bool UnCondition { get; set; } = true;
        public DateTime? ActivityStartDate { get; set; }
        public DateTime? ActivityEndDate { get; set; }
        public FreebieOrderReach? FreebieOrderReach { get; set; }
        public decimal MinimumAmount { get; set; }
        public int MinimumPiece { get; set; }
        public decimal FreebieAmount { get; set; }
        public Guid? TenantId { get; set; }
    }
}
