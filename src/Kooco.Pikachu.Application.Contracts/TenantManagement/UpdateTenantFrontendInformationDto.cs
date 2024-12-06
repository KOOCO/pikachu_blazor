using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateTenantFrontendInformationDto
{
    [Required]
    [MaxLength(TenantSettingsConsts.MaxWebpageTitleLength)]
    public string? WebpageTitle { get; set; }

    [MaxLength(TenantSettingsConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
    public string? FaviconBase64 { get; set; }
    public string? FaviconUrl { get; set; }
    public string? FaviconName { get; set; }

    public string? LogoBase64 { get; set; }
    public string? LogoUrl { get; set; }
    public string LogoName { get; set; }

    public string? BannerBase64 { get; set; }
    public string? BannerUrl { get; set; }
    public string? BannerName { get; set; }
}
