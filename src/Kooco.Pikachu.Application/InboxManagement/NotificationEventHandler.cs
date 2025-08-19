using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationEventHandler : ILocalEventHandler<NotificationCreatedEvent>, ILocalEventHandler<NotificationReadChangedEvent>, ITransientDependency
{
    protected ICurrentTenant CurrentTenant { get; }
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;

    public NotificationEventHandler(
        ICurrentTenant currentTenant,
        IHubContext<NotificationHub> hubContext,
        INotificationRepository notificationRepository
        )
    {
        CurrentTenant = currentTenant;
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
        var count = await _notificationRepository.CountUnreadAsync();
        var group = NotificationKeys.NotificationGroup(CurrentTenant?.Id);
        await _hubContext.Clients
            .Group(group)
            .SendAsync(NotificationKeys.UnreadCount, count);
    }
}