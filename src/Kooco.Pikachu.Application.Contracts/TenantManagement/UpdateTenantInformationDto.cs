using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateTenantInformationDto
{
    [Required]
    [RegularExpression("^[A-Za-z0-9]{8}$", ErrorMessage = "ShortCodeValidationMessage")]
    public string? ShortCode { get; set; }

    [Required]
    [Url]
    public string? TenantUrl { get; set; }

    [Required]
    [Url]
    public string? Domain { get; set; }

    [Required]
    public string? TenantContactTitle { get; set; }

    [Required]
    public string? TenantContactPerson { get; set; }

    [Required]
    [MaxLength(TenantSettingsConsts.MaxContactPhoneLength)]
    public string? ContactPhone { get; set; }

    [Required]
    [EmailAddress]
    public string? TenantContactEmail { get; set; }
}