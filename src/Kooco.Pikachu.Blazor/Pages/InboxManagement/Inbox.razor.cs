using Kooco.Pikachu.InboxManagement;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InboxManagement;

public partial class Inbox
{
    [CascadingParameter] public CancellationToken PageCancellationToken { get; set; }
    private List<NotificationDto> Notifications { get; set; } = [];
    private NotificationFilter Filter { get; set; } = NotificationFilter.All;
    private long TotalCount { get; set; }
    private bool Loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetNotificationsAsync();
    }

    async Task GetNotificationsAsync()
    {
        Loading = true;
        var data = await NotificationAppService.GetListAsync(
            new GetNotificationListInput
            {
                MaxResultCount = 10,
                SkipCount = Notifications?.Count ?? 0,
                Sorting = NotificationConsts.DefaultSorting,
                Filter = Filter
            }, PageCancellationToken);

        Notifications.AddRange([.. data.Items]);
        TotalCount = data.TotalCount;
        Loading = false;
    }

    Task OnRefreshAsync()
    {
        Notifications = [];
        return GetNotificationsAsync();
    }

    static string RowClass(NotificationDto notification)
    {
        string baseClass = "notification-row" + " ";

        return baseClass + (notification.IsRead
            ? "notification-read"
            : "notification-unread");
    }

    async Task MarkAllReadAsync()
    {
        await NotificationAppService.MarkAllReadAsync(PageCancellationToken);
        await GetNotificationsAsync();
    }

    async Task SetIsReadAsync(NotificationDto notification, bool isRead)
    {
        if (notification.IsRead == isRead)
        {
            return;
        }

        var updatedRecord = await NotificationAppService.SetIsReadAsync(notification.Id, isRead, PageCancellationToken);

        var index = Notifications.FindIndex(n => n.Id == notification.Id);
        if (index != -1)
        {
            Notifications[index] = updatedRecord;
        }
    }

    async Task OnToggleRead((NotificationDto Notification, bool IsRead) args)
    {
        await SetIsReadAsync(args.Notification, args.IsRead);
    }

    async Task OnView(NotificationDto notification)
    {
        await SetIsReadAsync(notification, true).ConfigureAwait(false);
    }
}