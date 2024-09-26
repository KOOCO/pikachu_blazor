using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public class CreateUserCumulativeFinancialDto
{
    [Required]
    public Guid? UserId { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalSpent { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalPaid { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalUnpaid { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalRefunded { get; set; }
}