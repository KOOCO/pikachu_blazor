using System.ComponentModel.DataAnnotations;
using System;

namespace Kooco.Pikachu.UserCumulativeFinancials;

public class UpdateUserCumulativeFinancialDto
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