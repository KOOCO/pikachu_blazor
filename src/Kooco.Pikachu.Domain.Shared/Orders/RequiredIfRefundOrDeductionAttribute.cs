using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.Orders
{
    public class RequiredIfRefundOrDeductionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the object containing the properties (the model)
            var instance = validationContext.ObjectInstance;

            // Use reflection to get the property values
            var refundAmount = (int)instance.GetType().GetProperty("RefundAmount")?.GetValue(instance);
            var refundRecordId = (Guid?)instance.GetType().GetProperty("RefundRecordId")?.GetValue(instance);
            var creditDeductionAmount = (int)instance.GetType().GetProperty("CreditDeductionAmount")?.GetValue(instance);
            var creditDeductionRecordId = (Guid?)instance.GetType().GetProperty("CreditDeductionRecordId")?.GetValue(instance);

            // Check if refund and deduction fields have values
            if (refundAmount > 0 && refundRecordId.HasValue && creditDeductionAmount > 0 && creditDeductionRecordId.HasValue)
            {
                // If UserId is null, validation fails
                if (value == null)
                {
                    return new ValidationResult("UserId is required when refund and deduction amounts are provided.");
                }
            }

            // Return success if validation passes
            return ValidationResult.Success;
        }
    }
}
