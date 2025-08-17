using Kooco.Pikachu.InboxManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
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

    async Task GetNotificationsAsync(bool loadMore = false)
    {
        try
        {
            Loading = true;

            if (!loadMore) Notifications = [];

            var data = await NotificationAppService.GetListAsync(
                new GetNotificationListInput
                {
                    MaxResultCount = 10,
                    SkipCount = loadMore ? Notifications?.Count ?? 0 : 0,
                    Sorting = NotificationConsts.DefaultSorting,
                    Filter = Filter
                }, PageCancellationToken);

            Notifications.AddRange([.. data.Items]);
            TotalCount = data.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    async Task MarkAllReadAsync()
    {
        try
        {
            Notifications = [];
            Loading = true;
            await NotificationAppService.MarkAllReadAsync(PageCancellationToken);
            await GetNotificationsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            Loading = false;
        }
    }

    async Task SetIsReadAsync(NotificationDto notification, bool isRead)
    {
        try
        {
            if (notification.IsRead == isRead)
            {
                return;
            }

            notification.Loading = true;

            var updatedRecord = await NotificationAppService.SetIsReadAsync(notification.Id, isRead, PageCancellationToken);

            var index = Notifications.FindIndex(n => n.Id == notification.Id);
            if (index != -1)
            {
                Notifications[index] = updatedRecord;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            notification.Loading = false;
        }
    }

    async Task OnToggleRead((NotificationDto Notification, bool IsRead) args)
    {
        await SetIsReadAsync(args.Notification, args.IsRead);
    }

    async Task OnView(NotificationDto notification)
    {
        try
        {
            await SetIsReadAsync(notification, true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }
}