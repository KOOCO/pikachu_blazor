using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderMessageAppService:IApplicationService
    {
        Task<OrderMessageDto> GetAsync(Guid id);
        Task<PagedResultDto<OrderMessageDto>> GetListAsync(GetOrderMessageListDto input);
        Task<OrderMessageDto> CreateAsync(CreateUpdateOrderMessageDto input);
        Task<OrderMessageDto> UpdateAsync(Guid id, CreateUpdateOrderMessageDto input);
        Task DeleteAsync(Guid id);
    }
}
