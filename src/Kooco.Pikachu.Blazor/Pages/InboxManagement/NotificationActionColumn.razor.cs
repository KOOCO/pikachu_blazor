using Kooco.Pikachu.InboxManagement;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Kooco.Pikachu.InboxManagement.NotificationParams;
using static Kooco.Pikachu.InboxManagement.NotificationType;

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
        Order or BankTransfer or Payment or OrderMessage => OrderDetails,
        Refund => RefundList,
        Return or Exchange => ReturnExchangeList,
        _ => ""
    };

    string OrderDetails => !string.IsNullOrWhiteSpace(P(OrderId)) ? string.Format("/Orders/OrderDetails/{0}", P(OrderId)) : "/Orders";
    static string RefundList => "/Refund";
    static string ReturnExchangeList => "/Orders/ReturnAndExchangeOrder";
    string P(string key) => Notification.Parameters.GetValueOrDefault(key) ?? string.Empty;
}