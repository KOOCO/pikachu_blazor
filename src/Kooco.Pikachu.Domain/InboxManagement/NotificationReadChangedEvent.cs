using System;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationReadChangedEvent
{
    public Guid NotificationId { get; }
    public bool IsRead { get; }

    public NotificationReadChangedEvent(Guid notificationId, bool isRead)
    {
        NotificationId = notificationId;
        IsRead = isRead;
    }
}
