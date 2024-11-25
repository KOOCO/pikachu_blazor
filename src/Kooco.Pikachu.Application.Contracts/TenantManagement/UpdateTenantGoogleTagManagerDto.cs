using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantManagement;

public class UpdateTenantGoogleTagManagerDto
{
    public bool GtmEnabled { get; set; }

    [MaxLength(TenantSettingsConsts.MaxGtmContainerIdLength)]
    public string? GtmContainerId { get; set; }
}
