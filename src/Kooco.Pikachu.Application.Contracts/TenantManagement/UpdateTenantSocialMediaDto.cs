using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateTenantSocialMediaDto
{
    [Url]
    public string? Facebook { get; set; }

    [Url]
    public string? Instagram { get; set; }

    [Url]
    public string? Line { get; set; }
}