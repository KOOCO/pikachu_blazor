using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.InboxManagement;

public interface INotificationAppService : IApplicationService
{
    Task<PagedResultDto<NotificationDto>> GetListAsync(GetNotificationListInput input, CancellationToken cancellationToken = default);
}