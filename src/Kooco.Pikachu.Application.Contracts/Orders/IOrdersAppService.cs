using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    public interface IOrdersAppService: ICrudAppService<
        OrderDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateOrderDto>
    {


    }
}
