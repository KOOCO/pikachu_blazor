using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.InboxManagement;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Task MarkAllReadAsync(Guid? userId, CancellationToken cancellationToken = default);
}
