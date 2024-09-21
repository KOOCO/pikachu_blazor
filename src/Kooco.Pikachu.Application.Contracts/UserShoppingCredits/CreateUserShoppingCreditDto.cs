using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserShoppingCredits;

public class CreateUserShoppingCreditDto
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

    public bool IsActive { get; set; } = true;
}

public class MustBeGreaterThanTodayAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime expirationDate)
        {
            if (expirationDate <= DateTime.Today)
            {
                return new ValidationResult("The expiration date must be greater than today's date.");
            }
        }

        return ValidationResult.Success;
    }
}
