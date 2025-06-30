using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.ProductCategories
{
    public class RequireOneOfAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyName;

        public RequireOneOfAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyName);

            if (otherProperty == null)
            {
                return new ValidationResult($"Unknown property: {_otherPropertyName}");
            }

            var otherValue = otherProperty.GetValue(validationContext.ObjectInstance) as string;
            var thisValue = value as string;

            if (string.IsNullOrWhiteSpace(thisValue) && string.IsNullOrWhiteSpace(otherValue))
            {
                return new ValidationResult($"Either {validationContext.MemberName} or {_otherPropertyName} must be provided.");
            }

            return ValidationResult.Success;
        }
    }
}
