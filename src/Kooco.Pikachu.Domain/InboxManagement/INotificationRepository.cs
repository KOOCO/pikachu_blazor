using System;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.InboxManagement;

public interface INotificationRepository : IRepository<Notification, Guid>
{
}
