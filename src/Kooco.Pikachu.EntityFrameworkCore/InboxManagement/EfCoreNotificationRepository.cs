using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.InboxManagement;

public class EfCoreNotificationRepository : EfCoreRepository<PikachuDbContext, Notification, Guid>, INotificationRepository
{
    public EfCoreNotificationRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> LongCountAsync(
        NotificationFilter filter = NotificationFilter.All, 
        CancellationToken cancellationToken = default
        )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var queryable = await GetFilteredQueryableAsync(filter, asNoTracking: true, cancellationToken);
        return await queryable.LongCountAsync(cancellationToken);
    }

    public async Task<List<Notification>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = 10, 
        string? sorting = null, 
        NotificationFilter filter = NotificationFilter.All,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryable = await GetFilteredQueryableAsync(filter, asNoTracking, cancellationToken);
        
        return await queryable
            .OrderBy(string.IsNullOrWhiteSpace(sorting) 
                ? NotificationConsts.DefaultSorting 
                : sorting
                )
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IQueryable<Notification>> GetFilteredQueryableAsync(
        NotificationFilter filter = NotificationFilter.All,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryable = (await GetQueryableAsync()).AsNoTrackingIf(asNoTracking);

        return queryable
            .WhereIf(filter == NotificationFilter.Unread, x => !x.IsRead)
            .WhereIf(filter == NotificationFilter.Today, x => x.CreationTime.Date == DateTime.Today)
            .WhereIf(filter == NotificationFilter.Orders, x => x.Type == NotificationType.Order)
            .Include(q => q.ReadBy);
    }

    public async Task MarkAllReadAsync(Guid? userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

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
