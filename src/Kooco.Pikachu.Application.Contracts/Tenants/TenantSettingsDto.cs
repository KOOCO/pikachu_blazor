using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants;

public class TenantSettingsDto : FullAuditedEntityDto<Guid>
{
    public string? WebpageTitle { get; set; }
    public string? Description { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? PrivacyPolicy { get; set; }
    public string? CompanyName { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public string? ContactPhone { get; set; }
    public string? CustomerServiceEmail { get; set; }
    public DateTime? ServiceHoursFrom { get; set; }
    public DateTime? ServiceHoursTo { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? TenantOwner { get; set; }
    public string? TenantContactTitle { get; set; }
    public string? TenantContactPerson { get; set; }
    public string? TenantContactEmail { get; set; }
    public string? TenantUrl { get; set; }
    public string? Domain { get; set; }
    public string? ShortCode { get; set; }
    public int? ShareProfitPercent { get; set; }
    public TenantStatus? Status { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? Line { get; set; }
    // Google Tag Manager
    public bool GtmEnabled { get; set; }
    public string? GtmContainerId { get; set; }
    public string? FaviconUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public TenantDto? Tenant { get; set; }
}