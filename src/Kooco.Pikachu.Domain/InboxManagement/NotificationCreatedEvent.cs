using System;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationCreatedEvent
{
    public Notification Notification { get; }
    public NotificationCreatedEvent(Notification notification)
    {
        Notification = notification;
    }
}
