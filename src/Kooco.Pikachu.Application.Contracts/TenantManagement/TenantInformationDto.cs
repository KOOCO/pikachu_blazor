using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class TenantInformationDto
{
    [Required]
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