using System.ComponentModel.DataAnnotations;
using System;

namespace Kooco.Pikachu.UserCumulativeOrders;

public class UpdateUserCumulativeOrderDto
{
    [Required]
    public Guid? UserId { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalOrders { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalExchanges { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalReturns { get; set; }
}