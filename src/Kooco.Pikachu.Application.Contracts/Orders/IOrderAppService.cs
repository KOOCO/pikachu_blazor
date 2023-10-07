using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderAppService: IApplicationService
    {
        Task<OrderDto> GetAsync(Guid id);
        Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input);
        Task DeleteAsync(Guid id);
        Task<OrderDto> CreateAsync(CreateOrderDto input);
    }
}
