using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Freebies.Dtos
{
    public class FreebieDto : FullAuditedEntityDto<Guid>
    {
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public bool ApplyToAllGroupBuy { get; set; }
        public ICollection<ImageDto> Images { get; set; }
        public ICollection<FreebieGroupBuysDto> FreebieGroupBuys { get; set; }
        public bool UnCondition { get; set; }
        public DateTime? ActivityStartDate { get; set; }
        public DateTime? ActivityEndDate { get; set; }
        public FreebieOrderReach? FreebieOrderReach { get; set; }
        public decimal MinimumAmount { get; set; }
        public int MinimumPiece { get; set; }
        public decimal FreebieAmount { get; set; }
        public Guid? TenantId { get; set; }
        public bool IsSelected { get; set; } = false;
        public bool IsFreebieAvaliable { get; set; }
    }
}
