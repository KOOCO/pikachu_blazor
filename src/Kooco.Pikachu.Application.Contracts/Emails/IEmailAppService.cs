using Kooco.Pikachu.OrderDeliveries;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Emails;

public interface IEmailAppService : IApplicationService
{
    Task SendLogisticsEmailAsync(OrderDeliveryDto input);
}
