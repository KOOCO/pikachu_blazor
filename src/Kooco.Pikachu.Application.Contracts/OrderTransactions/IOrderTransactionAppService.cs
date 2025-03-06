using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.OrderTransactions;

public interface IOrderTransactionAppService : IApplicationService
{
    Task<PagedResultDto<OrderTransactionDto>> GetListAsync(GetOrderTransactionListDto input);
}
