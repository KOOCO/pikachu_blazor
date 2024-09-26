using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public class UserCumulativeFinancialDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public int TotalSpent { get; set; }
    public int TotalPaid { get; set; }
    public int TotalUnpaid { get; set; }
    public int TotalRefunded { get; set; }
}