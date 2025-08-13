using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.InboxManagement;

public class NotificationAppService : PikachuAppService, INotificationAppService
{
    private readonly NotificationManager _notificationManager;
    private readonly INotificationRepository _notificationRepository;

    public NotificationAppService(
        NotificationManager notificationManager,
        INotificationRepository notificationRepository)
    {
        _notificationManager = notificationManager;
        _notificationRepository = notificationRepository;
    }

    public async Task<PagedResultDto<NotificationDto>> GetListAsync(GetNotificationListInput input, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // TODO: Implement list and count inside repository
        var queryable = await _notificationRepository.GetQueryableAsync();
        return new PagedResultDto<NotificationDto>(
            await queryable.CountAsync(cancellationToken),
            await queryable
                .Include(n => n.ReadBy)
                .OrderBy(NotificationConsts.DefaultSorting)
                .PageBy(input)
                .Select(n => ObjectMapper.Map<Notification, NotificationDto>(n))
                .ToListAsync(cancellationToken)
        );
    }

    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _notificationRepository.MarkAllReadAsync(CurrentUser.Id, cancellationToken);
    }

    public async Task<NotificationDto> SetIsReadAsync(Guid id, bool isRead, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var notification = await _notificationRepository.GetAsync(id, cancellationToken: cancellationToken);
        notification.SetIsRead(isRead, CurrentUser.Id);
        await _notificationRepository.UpdateAsync(notification, cancellationToken: cancellationToken);
        return ObjectMapper.Map<Notification, NotificationDto>(notification);
    }
}
