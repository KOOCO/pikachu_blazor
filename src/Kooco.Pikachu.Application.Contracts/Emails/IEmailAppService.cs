﻿using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Emails;

public interface IEmailAppService : IApplicationService
{
    Task SendLogisticsEmailAsync(Guid orderId, string? deliveryNo = "");
    Task SendOrderStatusEmailAsync(Guid id, OrderStatus? orderStatus = null);
}
