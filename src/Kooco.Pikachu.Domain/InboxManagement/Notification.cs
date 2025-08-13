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
    public string? TitleParamsJson { get; private set; }
    public string? MessageParamsJson { get; private set; }
    public string? UrlParamsJson { get; private set; }
    public NotificationEntityType EntityType { get; private set; }
    public string? EntityId { get; private set; }
    public Guid? TenantId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadTime { get; set; }
    public Guid? ReadById { get; set; }
    public DateTime NotificationTimeUtc { get; set; }

    private Dictionary<string, string>? _titleParams;
    public Dictionary<string, string> TitleParams => _titleParams ??= DeserializeParams(TitleParamsJson);

    private Dictionary<string, string>? _messageParams;
    public Dictionary<string, string> MessageParams => _messageParams ??= DeserializeParams(MessageParamsJson);

    private Dictionary<string, string>? _urlParams;
    public Dictionary<string, string> UrlParams => _urlParams ??= DeserializeParams(UrlParamsJson);

    private static Dictionary<string, string> DeserializeParams(string? json) =>
        !string.IsNullOrWhiteSpace(json)
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? []
            : [];

    public virtual IdentityUser ReadBy { get; set; }

    private Notification()
    {
        // Required by EF Core
    }

    public Notification(
        Guid id,
        NotificationType type,
        string title,
        string? message,
        Dictionary<string, string>? titleParams,
        Dictionary<string, string>? messageParams,
        Dictionary<string, string>? urlParams,
        NotificationEntityType entityType,
        string? entityId
    ) : base(id)
    {
        Type = type;
        SetTitle(title);
        SetMessage(message);
        SetTitleParams(titleParams);
        SetMessageParams(messageParams);
        SetUrlParams(urlParams);
        SetEntity(entityType, entityId);
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

    internal void SetTitleParams(Dictionary<string, string>? titleParams)
    {
        TitleParamsJson = titleParams != null
            ? Check.Length(
                JsonSerializer.Serialize(titleParams),
                nameof(MessageParamsJson),
                maxLength: NotificationConsts.MaxParamsJsonLength
            )
            : null;
    }

    internal void SetMessageParams(Dictionary<string, string>? messageParams)
    {
        MessageParamsJson = messageParams != null
            ? Check.Length(
                JsonSerializer.Serialize(messageParams),
                nameof(MessageParamsJson),
                maxLength: NotificationConsts.MaxParamsJsonLength
            )
            : null;
    }

    internal void SetUrlParams(Dictionary<string, string>? urlParams)
    {
        UrlParamsJson = urlParams != null
            ? Check.Length(
                JsonSerializer.Serialize(urlParams),
                nameof(MessageParamsJson),
                maxLength: NotificationConsts.MaxParamsJsonLength
            )
            : null;
    }

    internal void SetEntity(NotificationEntityType entityType, string? entityId)
    {
        EntityType = entityType;
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