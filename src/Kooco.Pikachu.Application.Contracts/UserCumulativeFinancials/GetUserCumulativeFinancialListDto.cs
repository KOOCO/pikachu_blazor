using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public class GetUserCumulativeFinancialListDto : PagedAndSortedResultRequestDto
{
    public Guid? UserId { get; set; }
    public int? MinTotalSpent { get; set; }
    public int? MaxTotalSpent { get; set; }
    public int? MinTotalPaid { get; set; }
    public int? MaxTotalPaid { get; set; }
    public int? MinTotalUnpaid { get; set; }
    public int? MaxTotalUnpaid { get; set; }
    public int? MinTotalRefunded { get; set; }
    public int? MaxTotalRefunded { get; set; }
}