using Microsoft.EntityFrameworkCore;
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
}
