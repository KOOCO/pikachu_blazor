using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class GetMemberMessageListDto : PagedAndSortedResultRequestDto
{
    public bool? IsRead { get; set; } = false;
    public List<Guid> OrderIds { get; set; } = [];
    public Guid? GroupBuyId { get; set; }
    public bool? IsMerchant { get; set; }
}
