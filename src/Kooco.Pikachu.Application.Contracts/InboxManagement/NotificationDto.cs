using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationDto : CreationAuditedEntity<Guid>, IMultiTenant
{
    public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string? Message { get; set; }
    public string? TitleParamsJson { get; set; }
    public string? MessageParamsJson { get; set; }
    public string? UrlParamsJson { get; set; }
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public Guid? TenantId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadTime { get; set; }
    public Guid? ReadById { get; set; }
    public string? ReadByName { get; set; }
    public DateTime NotificationTimeUtc { get; set; }
    public Dictionary<string, string> TitleParams { get; set; }
    public Dictionary<string, string> MessageParams { get; set; }
    public Dictionary<string, string> UrlParams { get; set; }
}
