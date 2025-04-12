using Kooco.Pikachu.TenantManagement;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Tenants;

public class UpdateTenantGoogleTagManagerDto
{
    public bool GtmEnabled { get; set; }

    [MaxLength(TenantSettingsConsts.MaxGtmContainerIdLength)]
    public string? GtmContainerId { get; set; }
}
