using Kooco.Pikachu.InboxManagement;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InboxManagement;

public partial class Inbox
{
    [CascadingParameter] public CancellationToken PageCancellationToken { get; set; }
    private IReadOnlyList<NotificationDto> Notifications { get; set; } = [];
    private long TotalCount { get; set; }
    private bool Loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetNotificationsAsync();
    }

    async Task GetNotificationsAsync(NotificationFilter? filter = null)
    {
        filter ??= NotificationFilter.All;
        Loading = true;
        var data = await NotificationAppService.GetListAsync(
            new GetNotificationListInput
            {
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = NotificationConsts.DefaultSorting,
                Filter = filter.Value
            }, PageCancellationToken);

        Notifications = data.Items;
        TotalCount = data.TotalCount;
        Loading = false;
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
        notification.IsRead = updatedRecord.IsRead;
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