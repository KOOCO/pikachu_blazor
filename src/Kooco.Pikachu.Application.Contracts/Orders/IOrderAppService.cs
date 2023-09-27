using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderAppService: ICrudAppService<
        OrderDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateOrderDto>, IApplicationService
    {


    }
}
