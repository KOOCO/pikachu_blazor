using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateTenantCustomerServiceDto
{
    [Required]
    [MaxLength(TenantSettingsConsts.MaxCompanyNameLength)]
    public string? CompanyName { get; set; }

    [Required]
    [MaxLength(TenantSettingsConsts.MaxBusinessRegistrationNumberLength)]
    public string? BusinessRegistrationNumber { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(TenantSettingsConsts.MaxCustomerServiceEmailLength)]
    public string? CustomerServiceEmail { get; set; }
    
    [Required]
    [MaxLength(TenantSettingsConsts.MaxContactPhoneLength)]
    public string? CustomerServiceContactPhone { get; set; }

    [Required]
    public DateTime? ServiceHoursFrom { get; set; }

    [Required]
    public DateTime? ServiceHoursTo { get; set; }
}