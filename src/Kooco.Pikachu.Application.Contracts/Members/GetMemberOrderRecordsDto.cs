using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class GetMemberOrderRecordsDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? GroupBuyId { get; set; }
    public List<Guid>? OrderIds { get; set; } = [];
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public ShippingStatus? ShippingStatus { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
}