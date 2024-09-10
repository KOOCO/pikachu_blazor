using Kooco.Pikachu.EnumValues;
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
        Task<RefundDto> UpdateRefundReviewAsync(Guid id, RefundReviewStatus input);
        Task SendRefundRequestAsync(Guid id);

        Task CheckStatusAndRequestRefundAsync(Guid id);
    }
}
