using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.OrderDeliveries
{
    public interface IOrderDeliveryAppService : IApplicationService
    {
        Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid Id);

    }
}
