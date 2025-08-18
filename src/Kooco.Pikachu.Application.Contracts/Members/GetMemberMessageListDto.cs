using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class GetMemberMessageListDto : PagedAndSortedResultRequestDto
{
    public bool? IsRead { get; set; } = false;
    public Guid? OrderId { get; set; }
    public bool? IsMerchant { get; set; }
}
