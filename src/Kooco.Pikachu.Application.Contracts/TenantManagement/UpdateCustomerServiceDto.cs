using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateCustomerServiceDto
{
    [Required]
    [RegularExpression("^[A-Za-z0-9]{8}$", ErrorMessage = "ShortCodeValidationMessage")]
    public string? ShortCode { get; set; }

    [Required]
    [MaxLength(TenantSettingsConsts.MaxCompanyNameLength)]
    public string? CompanyName { get; set; }

    [Required]
    [MaxLength(TenantSettingsConsts.MaxBusinessRegistrationNumberLength)]
    public string? BusinessRegistrationNumber { get; set; }

    [Required]
    [MaxLength(TenantSettingsConsts.MaxContactPhoneLength)]
    public string? ContactPhone { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(TenantSettingsConsts.MaxCustomerServiceEmailLength)]
    public string? CustomerServiceEmail { get; set; }

    [Required]
    public DateTime? ServiceHoursFrom { get; set; }

    [Required]
    public DateTime? ServiceHoursTo { get; set; }
}