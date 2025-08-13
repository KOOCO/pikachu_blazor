using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.InboxManagement;

public class EfCoreNotificationRepository : EfCoreRepository<PikachuDbContext, Notification, Guid>, INotificationRepository
{
    public EfCoreNotificationRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task MarkAllReadAsync(Guid? userId, CancellationToken cancellationToken = default)
    {
        var queryable = (await GetQueryableAsync())
            .Where(q => !q.IsRead)
            .AsNoTracking();

        await queryable
            .Where(q => !q.IsRead)
            .ExecuteUpdateAsync(q => q
                .SetProperty(p => p.IsRead, true)
                .SetProperty(p => p.ReadById, userId)
                .SetProperty(p => p.ReadTime, DateTime.Now),
                cancellationToken);
    }
}
