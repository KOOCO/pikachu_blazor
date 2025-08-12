using Kooco.Pikachu.InboxManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InboxManagement;

public partial class Inbox
{
    private IReadOnlyList<NotificationDto> Notifications { get; set; } = [];
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
                SkipCount = 0,
                Sorting = NotificationConsts.DefaultSorting
            });

        Notifications = data.Items;
        TotalCount = data.TotalCount;
        Loading = false;
    }

    string RowClass(NotificationDto notification)
    {
        string baseClass = "notification-row" + " ";

        return baseClass + (notification.IsRead
            ? "notification-read"
            : "notification-unread");
    }
}