using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeOrders;

public class GetUserCumulativeOrderListDto : PagedAndSortedResultRequestDto
{
    public Guid? UserId { get; set; }
    public int? MinTotalOrders { get; set; }
    public int? MaxTotalOrders { get; set; }
    public int? MinTotalExchanges { get; set; }
    public int? MaxTotalExchanges { get; set; }
    public int? MinTotalReturns { get; set; }
    public int? MaxTotalReturns { get; set; }
}