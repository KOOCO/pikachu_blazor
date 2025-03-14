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
    [HttpGet("check-status-and-request-refund")]
    public Task CheckStatusAndRequestRefundAsync(Guid id)
    {
        throw new NotImplementedException();
    }

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
    [HttpGet]
    public Task<long> GetRefundPendingCount()
    {
        return _refundAppService.GetRefundPendingCount();
    }

    [HttpPost("send-refund-request")]
    public Task SendRefundRequestAsync(Guid id)
    {
        return _refundAppService.SendRefundRequestAsync(id);
    }

    [HttpPut("{id}/{input}")]
    public Task<RefundDto> UpdateRefundReviewAsync(Guid id, RefundReviewStatus input, string? rejectReason = null)
    {
        return _refundAppService.UpdateRefundReviewAsync(id, input);
    }
}
