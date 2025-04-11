using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Orders.Interfaces
{
    public interface IOrderMessageAppService:IApplicationService
    {
        Task<OrderMessageDto> GetAsync(Guid id);
        Task<PagedResultDto<OrderMessageDto>> GetListAsync(GetOrderMessageListDto input);
        Task<List<OrderMessageDto>> GetOrderMessagesAsync(Guid orderId);
        Task<OrderMessageDto> CreateAsync(CreateUpdateOrderMessageDto input);
        Task<OrderMessageDto> UpdateAsync(Guid id, CreateUpdateOrderMessageDto input);
        Task DeleteAsync(Guid id);
    }
}
