using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class UserCumulativeCreditDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public int TotalAmount { get; private set; }
    public int TotalDeductions { get; private set; }
    public int TotalRefunds { get; private set; }
}