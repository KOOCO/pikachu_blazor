using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationHub : AbpHub
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationHub(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<long> GetUnreadCountAsync()
    {
        return await _notificationRepository.LongCountAsync(NotificationFilter.Unread);
    }
}