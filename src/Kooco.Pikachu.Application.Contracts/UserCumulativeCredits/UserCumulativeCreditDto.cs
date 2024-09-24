using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class UserCumulativeCreditDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public int TotalAmount { get; set; }
    public int TotalDeductions { get; set; }
    public int TotalRefunds { get; set; }
}