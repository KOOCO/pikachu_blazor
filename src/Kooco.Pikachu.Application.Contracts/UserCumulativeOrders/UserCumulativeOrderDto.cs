using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserCumulativeOrders;

public class UserCumulativeOrderDto : CreationAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public int TotalOrders { get; set; }
    public int TotalExchanges { get; set; }
    public int TotalReturns { get; set; }
}