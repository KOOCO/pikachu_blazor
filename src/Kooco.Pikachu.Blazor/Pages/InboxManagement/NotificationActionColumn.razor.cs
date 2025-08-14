using Kooco.Pikachu.InboxManagement;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InboxManagement;

public partial class NotificationActionColumn
{
    [Parameter] public NotificationDto Notification { get; set; }
    [Parameter] public EventCallback<NotificationDto> OnView { get; set; }
    [Parameter] public EventCallback<(NotificationDto Notification, bool IsRead)> OnToggleRead { get; set; }

    Task OnViewClicked() => OnView.InvokeAsync(Notification);

    Task ToggleIsRead(bool isRead) => OnToggleRead.InvokeAsync((Notification, isRead));

    string Url => Notification.Type switch
    {
        NotificationType.Order or NotificationType.ManualBankTransfer 
            => "Orders/OrderDetails/" + Notification.UrlParams.GetValueOrDefault(NotificationParams.Orders.OrderId),
        _ => ""
    };
}