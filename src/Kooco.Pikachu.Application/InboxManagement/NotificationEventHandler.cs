using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationEventHandler : ILocalEventHandler<NotificationCreatedEvent>, ILocalEventHandler<NotificationReadChangedEvent>, ITransientDependency
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;
    public NotificationEventHandler(
        IHubContext<NotificationHub> hubContext,
        INotificationRepository notificationRepository
        )
    {
        _hubContext = hubContext;
        _notificationRepository = notificationRepository;
    }

    public async Task HandleEventAsync(NotificationCreatedEvent eventData)
    {
        await PushUnreadCountAsync();
    }

    public async Task HandleEventAsync(NotificationReadChangedEvent eventData)
    {
        await PushUnreadCountAsync();
    }

    private async Task PushUnreadCountAsync()
    {
        var count = await _notificationRepository.LongCountAsync(NotificationFilter.Unread);
        await _hubContext.Clients.All.SendAsync(NotificationKeys.UnreadCount, count);
    }
}