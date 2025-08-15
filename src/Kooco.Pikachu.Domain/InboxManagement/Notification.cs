using System;
using System.Collections.Generic;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.InboxManagement;

public class Notification : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public NotificationType Type { get; set; }
    public string Title { get; private set; }
    public string? Message { get; private set; }
    public string? ParametersJson { get; private set; }
    public string? EntityName { get; private set; }
    public string? EntityId { get; private set; }
    public Guid? TenantId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadTime { get; set; }
    public Guid? ReadById { get; set; }
    public DateTime NotificationTimeUtc { get; set; }

    private Dictionary<string, string>? _parameters;
    public Dictionary<string, string> Parameters => _parameters ??= DeserializeParams(ParametersJson);

    private static Dictionary<string, string> DeserializeParams(string? json) =>
        !string.IsNullOrWhiteSpace(json)
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? []
            : [];

    public virtual IdentityUser ReadBy { get; set; }

    private Notification() { }

    internal Notification(
        Guid id,
        NotificationType type,
        string title,
        string? message = null,
        Dictionary<string, string>? parameters = null,
        string? entityName = null,
        string? entityId = null
    ) : base(id)
    {
        Type = type;
        SetTitle(title);
        SetMessage(message);
        SetParameters(parameters);
        SetEntity(entityName, entityId);
        NotificationTimeUtc = DateTime.UtcNow;
    }

    internal void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), maxLength: NotificationConsts.MaxTitleLength);
    }

    internal void SetMessage(string? message)
    {
        Message = Check.Length(message, nameof(message), maxLength: NotificationConsts.MaxMessageLength);
    }

    internal void SetParameters(Dictionary<string, string>? parameters)
    {
        ParametersJson = parameters != null
            ? Check.Length(
                JsonSerializer.Serialize(parameters),
                nameof(Parameters),
                maxLength: NotificationConsts.MaxParamsJsonLength
            )
            : null;
    }

    internal void SetEntity(string? entityName, string? entityId)
    {
        EntityName = Check.Length(entityName, nameof(entityName), maxLength: NotificationConsts.MaxEntityNameLength);
        EntityId = Check.Length(entityId, nameof(entityId), maxLength: NotificationConsts.MaxEntityIdLength);
    }

    public void SetIsRead(bool isRead, Guid? readById)
    {
        IsRead = isRead;
        if (IsRead)
        {
            ReadTime = DateTime.Now;
            ReadById = readById;
        }
        else
        {
            ReadTime = null;
            ReadById = null;
        }
    }
}