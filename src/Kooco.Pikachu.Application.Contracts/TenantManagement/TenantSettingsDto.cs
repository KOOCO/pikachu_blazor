using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.TenantManagement;

public class TenantSettingsDto : FullAuditedEntityDto<Guid>
{
    public string? FaviconUrl { get; set; }
    public string? WebpageTitle { get; set; }
    public string? PrivacyPolicy { get; set; }
    public string? CompanyName { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public string? ContactPhone { get; set; }
    public string? CustomerServiceEmail { get; set; }
    public DateTime? ServiceHoursFrom { get; set; }
    public DateTime? ServiceHoursTo { get; set; }
    public Guid? TenantId { get; set; }
    public TenantDto? Tenant { get; set; }
}