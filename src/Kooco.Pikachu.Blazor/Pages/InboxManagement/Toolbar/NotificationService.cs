using Kooco.Pikachu.InboxManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InboxManagement.Toolbar;

public class NotificationService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly INotificationRepository _notificationRepository;
    private readonly NavigationManager _navigationManager;
    private long _unreadCount = 0;
    private bool _isInitialized = false;

    public event Func<long, Task>? UnreadCountChanged;
    public long UnreadCount => _unreadCount;

    public NotificationService(
        INotificationRepository notificationRepository,
        NavigationManager navigationManager)
    {
        _notificationRepository = notificationRepository;
        _navigationManager = navigationManager;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/signalr-notifications"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<long>(NotificationKeys.UnreadCount, OnUnreadCountReceived);

        await _hubConnection.StartAsync();

        // Get initial unread count
        _unreadCount = await _notificationRepository.LongCountAsync(NotificationFilter.Unread);
        await NotifyUnreadCountChanged();

        _isInitialized = true;
    }

    private async Task OnUnreadCountReceived(long count)
    {
        _unreadCount = count;
        await NotifyUnreadCountChanged();
    }

    private async Task NotifyUnreadCountChanged()
    {
        if (UnreadCountChanged != null)
        {
            await UnreadCountChanged.Invoke(_unreadCount);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}