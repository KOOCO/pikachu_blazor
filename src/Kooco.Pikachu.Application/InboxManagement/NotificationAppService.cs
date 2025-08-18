using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationAppService : PikachuAppService, INotificationAppService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILocalEventBus _localEventBus;

    public NotificationAppService(
        INotificationRepository notificationRepository,
        ILocalEventBus localEventBus
        )
    {
        _notificationRepository = notificationRepository;
        _localEventBus = localEventBus;
    }

    public async Task<PagedResultDto<NotificationDto>> GetListAsync(GetNotificationListInput input, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var totalCount = await _notificationRepository.LongCountAsync(input.Filter, cancellationToken);

        var items = await _notificationRepository
            .GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter,
                asNoTracking: true,
                cancellationToken: cancellationToken
            );

        return new PagedResultDto<NotificationDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Notification>, List<NotificationDto>>(items)
        };
    }

    public async Task<NotificationsCountDto> GetNotificationsCountAsync(CancellationToken cancellationToken = default)
    {
        var counts = await _notificationRepository.GetNotificationsCountAsync(cancellationToken);
        return ObjectMapper.Map<NotificationsCountModel, NotificationsCountDto>(counts);
    }

    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _notificationRepository.MarkAllReadAsync(CurrentUser.Id, cancellationToken);
        await _localEventBus.PublishAsync(new NotificationReadChangedEvent(Guid.Empty, true));
    }

    public async Task<NotificationDto> SetIsReadAsync(Guid id, bool isRead, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var notification = await _notificationRepository.GetAsync(id, cancellationToken: cancellationToken);
        notification.SetIsRead(isRead, CurrentUser.Id);
        await _notificationRepository.UpdateAsync(notification, cancellationToken: cancellationToken);
        await _notificationRepository.EnsurePropertyLoadedAsync(notification, n => n.ReadBy, cancellationToken);
        return ObjectMapper.Map<Notification, NotificationDto>(notification);
    }
}
