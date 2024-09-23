using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserCumulativeCredits;

public class CreateUserCumulativeCreditDto
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