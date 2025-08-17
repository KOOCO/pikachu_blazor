using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace Kooco.Pikachu.InboxManagement.Managers;

public partial class NotificationManager : DomainService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationManager> _logger;
    private readonly ILocalEventBus _localEventBus;

    public NotificationManager(
        INotificationRepository notificationRepository,
        ILogger<NotificationManager> logger,
        ILocalEventBus localEventBus
        )
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
        _localEventBus = localEventBus;
    }

    public async Task<Notification> CreateAsync(
        NotificationType type,
        string title,
        string? message = null,
        Dictionary<string, string>? parameters = null,
        string? entityName = null,
        string? entityId = null
        )
    {
        var notification = new Notification(
            GuidGenerator.Create(),
            type,
            title,
            message,
            parameters,
            entityName,
            entityId
        );

        await _notificationRepository.InsertAsync(notification);
        await _localEventBus.PublishAsync(new NotificationCreatedEvent(notification));
        return notification;
    }

    public async Task SafeAsync(Func<NotificationManager, Task> action, string contextMessage = null!)
    {
        try
        {
            await action(this);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to save notification for {context}", contextMessage ?? string.Empty);
            _logger.LogException(ex);
        }
    }
}
