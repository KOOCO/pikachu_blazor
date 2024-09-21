using System.ComponentModel.DataAnnotations;
using System;

namespace Kooco.Pikachu.UserShoppingCredits;

public class UpdateUserShoppingCreditDto
{
    [Required]
    public Guid UserId { get; set; }

    [Range(0, int.MaxValue)]
    public int Amount { get; set; }

    [Range(0, int.MaxValue)]
    public int CurrentRemainingCredits { get; set; }

    [Required]
    [MaxLength(UserShoppingCreditConsts.MaxTransactionDescriptionLength)]
    public string? TransactionDescription { get; set; }

    [DataType(DataType.Date)]
    [MustBeGreaterThanToday(ErrorMessage = "MustBeGreaterThanToday")]
    public DateTime? ExpirationDate { get; set; }

    public bool IsActive { get; set; }
}