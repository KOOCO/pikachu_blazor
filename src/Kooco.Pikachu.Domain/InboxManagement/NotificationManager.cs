using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.InboxManagement;

public partial class NotificationManager : DomainService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationManager(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Notification> CreateAsync(
        NotificationType type,
        string title,
        string? message,
        Dictionary<string, string>? titleParams,
        Dictionary<string, string>? messageParams,
        Dictionary<string, string>? urlParams,
        NotificationEntityType entityType,
        string? entityId
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
            entityType,
            entityId
        );

        await _notificationRepository.InsertAsync(notification);
        return notification;
    }
}
