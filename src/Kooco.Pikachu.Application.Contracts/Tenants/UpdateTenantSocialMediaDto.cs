using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Tenants;

public class UpdateTenantSocialMediaDto
{
    
    public string? FacebookDisplayName { get; set; }
    [Url]
    public string? FacebookLink { get; set; }
    public string? InstagramDisplayName { get; set; }
    [Url]
    public string? InstagramLink { get; set; }
    public string? LineDisplayName { get; set; }
    [Url]
    public string? LineLink { get; set; }
}