using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class GetUserCumulativeCreditListDto : PagedAndSortedResultRequestDto
{
    public Guid? UserId { get; set; }
    public int? MinTotalAmount { get; set; }
    public int? MaxTotalAmount { get; set; }
    public int? MinTotalDeductions { get; set; }
    public int? MaxTotalDeductions { get; set; }
    public int? MinTotalRefunds { get; set; }
    public int? MaxTotalRefunds { get; set; }
}