using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.InboxManagement.Managers;

public partial class NotificationManager : DomainService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationManager> _logger;

    public NotificationManager(
        INotificationRepository notificationRepository,
        ILogger<NotificationManager> logger
        )
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<Notification> CreateAsync(
        NotificationType type,
        string title,
        string? message = null,
        Dictionary<string, string>? titleParams = null,
        Dictionary<string, string>? messageParams = null,
        Dictionary<string, string>? urlParams = null,
        string? entityName = null,
        string? entityId = null
        )
    {
        var notification = new Notification(
            GuidGenerator.Create(),
            type,
            title,
            message,
            titleParams,
            messageParams,
            urlParams,
            entityName,
            entityId
        );

        await _notificationRepository.InsertAsync(notification);
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
