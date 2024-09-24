using System.ComponentModel.DataAnnotations;
using System;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class UpdateUserCumulativeCreditDto
{
    [Required]
    public Guid? UserId { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalAmount { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalDeductions { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalRefunds { get; set; }
}