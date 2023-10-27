using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Refunds
{
    public interface IRefundAppService : IApplicationService
    {
        Task CreateAsync(Guid orderId);
        Task<PagedResultDto<RefundDto>> GetListAsync(GetRefundListDto input);
    }
}
