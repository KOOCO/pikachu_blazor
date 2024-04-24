using Asp.Versioning;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Refunds;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.Refunds;

[RemoteService(IsEnabled = true)]
[ControllerName("Refunds")]
[Area("app")]
[Route("api/app/refunds")]
public class RefundController(
    IRefundAppService _refundAppService
    ) : AbpController, IRefundAppService
{
    [HttpPost("{id}")]
    public Task CreateAsync(Guid orderId)
    {
        return _refundAppService.CreateAsync(orderId);
    }

    [HttpGet]
    public Task<PagedResultDto<RefundDto>> GetListAsync(GetRefundListDto input)
    {
        return _refundAppService.GetListAsync(input);
    }

    [HttpPut("{id}/{input}")]
    public Task<RefundDto> UpdateRefundReviewAsync(Guid id, RefundReviewStatus input)
    {
        return _refundAppService.UpdateRefundReviewAsync(id, input);
    }
}
