using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateTenantSettingsDto : IValidatableObject
{
    [MaxLength(TenantSettingsConsts.MaxFaviconUrlLength)]
    public string? FaviconUrl { get; set; }

    [MaxLength(TenantSettingsConsts.MaxWebpageTitleLength)]
    public string? WebpageTitle { get; set; }

    [MaxLength(TenantSettingsConsts.MaxPrivacyPolicyLength)]
    public string? PrivacyPolicy { get; set; }

    [MaxLength(TenantSettingsConsts.MaxCompanyNameLength)]
    public string? CompanyName { get; set; }

    [MaxLength(TenantSettingsConsts.MaxBusinessRegistrationNumberLength)]
    public string? BusinessRegistrationNumber { get; set; }

    [MaxLength(TenantSettingsConsts.MaxContactPhoneLength)]
    public string? ContactPhone { get; set; }

    [MaxLength(TenantSettingsConsts.MaxCustomerServiceEmailLength)]
    public string? CustomerServiceEmail { get; set; }

    public DateTime? ServiceHoursFrom { get; set; }

    public DateTime? ServiceHoursTo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ServiceHoursFrom.HasValue && ServiceHoursTo.HasValue)
        {
            if (ServiceHoursFrom >= ServiceHoursTo)
            {
                yield return new ValidationResult("Pikachu:InvalidServiceHoursException", [nameof(ServiceHoursTo)]);
            }
        }
    }
}