using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.InboxManagement;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Task<long> CountUnreadAsync(CancellationToken cancellationToken = default);

    Task<long> LongCountAsync(
        NotificationFilter filter = NotificationFilter.All,
        CancellationToken cancellationToken = default
        );
    
    Task<List<Notification>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = 10,
        string? sorting = default,
        NotificationFilter filter = NotificationFilter.All,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
        );

    Task<IQueryable<Notification>> GetFilteredQueryableAsync(
        NotificationFilter filter = NotificationFilter.All,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
        );

    Task<NotificationsCountModel> GetNotificationsCountAsync(CancellationToken cancellationToken = default);

    Task MarkAllReadAsync(Guid? userId, CancellationToken cancellationToken = default);
}
