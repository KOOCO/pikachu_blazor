using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserCumulativeOrders;

public class CreateUserCumulativeOrderDto
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