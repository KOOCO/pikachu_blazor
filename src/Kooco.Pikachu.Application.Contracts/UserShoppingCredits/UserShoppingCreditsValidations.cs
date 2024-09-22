using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserShoppingCredits;

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

public class ExpirationDateRequiredIfLimitedAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dto = (ICreateUpdateUserShoppingCreditDto)validationContext.ObjectInstance;

        if (dto.CanExpire == true && dto.ExpirationDate == null)
        {
            return new ValidationResult("Expiration date is required if CanExpire is true.");
        }

        return ValidationResult.Success;
    }
}
