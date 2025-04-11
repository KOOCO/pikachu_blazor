using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Tenants.Entities;
public class TenantEmailSettings : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string? SenderName { get; set; } 
    public string? Subject { get; set; }
    public string? Greetings { get; set; }
    public string? Footer { get; set; }
}