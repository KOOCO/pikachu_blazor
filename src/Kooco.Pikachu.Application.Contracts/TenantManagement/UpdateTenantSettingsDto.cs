using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateTenantSettingsDto
{
    [Required]
    [MaxLength(TenantSettingsConsts.MaxWebpageTitleLength)]
    public string? WebpageTitle { get; set; }

    [Required]
    public string? PrivacyPolicy { get; set; }

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

    [Required]
    public string? TenantContactTitle { get; set; }

    [Required]
    public string? TenantContactPerson { get; set; }

    [Required]
    [EmailAddress]
    public string? TenantContactEmail { get; set; }

    [Required]
    public string? Domain { get; set; }

    [Required]
    [RegularExpression("^[A-Za-z0-9]{8}$", ErrorMessage = "ShortCodeValidationMessage")]
    public string? ShortCode { get; set; }

    public string? FaviconBase64 { get; set; }
    public string? FaviconUrl { get; set; }
    public string? FaviconName { get; set; }

    public string? LogoBase64 { get; set; }
    public string? LogoUrl { get; set; }
    public string LogoName { get; set; }

    public string? BannerBase64 { get; set; }
    public string? BannerUrl { get; set; }
    public string? BannerName { get; set; }}