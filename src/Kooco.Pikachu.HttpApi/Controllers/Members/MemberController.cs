using Asp.Versioning;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.UserCumulativeOrders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.Members;

[ControllerName("Members")]
[RemoteService(IsEnabled = true)]
[Area("app")]
[Route("api/app/members")]
public class MemberController(IMemberAppService memberAppService) : PikachuController, IMemberAppService
{
    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return memberAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<MemberDto> GetAsync(Guid id)
    {
        return memberAppService.GetAsync(id);
    }

    [HttpGet("default-address/{id}")]
    public Task<UserAddressDto?> GetDefaultAddressAsync(Guid id)
    {
        return memberAppService.GetDefaultAddressAsync(id);
    }

    [HttpGet("groupbuy-lookup")]
    public Task<List<KeyValueDto>> GetGroupBuyLookupAsync()
    {
        return memberAppService.GetGroupBuyLookupAsync();
    }

    [HttpGet("list")]
    public Task<PagedResultDto<MemberDto>> GetListAsync(GetMemberListDto input)
    {
        return memberAppService.GetListAsync(input);
    }

    [HttpGet("{id}/credit_records")]
    public Task<PagedResultDto<MemberCreditRecordDto>> GetMemberCreditRecordAsync(Guid id, GetMemberCreditRecordListDto input)
    {
        return memberAppService.GetMemberCreditRecordAsync(id, input);
    }

    [HttpGet("{id}/cumulative_credits")]
    public Task<UserCumulativeCreditDto> GetMemberCumulativeCreditsAsync(Guid id)
    {
        return memberAppService.GetMemberCumulativeCreditsAsync(id);
    }

    [HttpGet("{id}/cumulative-orders")]
    public Task<UserCumulativeOrderDto> GetMemberCumulativeOrdersAsync(Guid id)
    {
        return memberAppService.GetMemberCumulativeOrdersAsync(id);
    }

    [HttpGet("{id}/cumulative-financials")]
    public Task<UserCumulativeFinancialDto> GetMemberCumulativeFinancialsAsync(Guid id)
    {
        return memberAppService.GetMemberCumulativeFinancialsAsync(id);
    }

    [HttpGet("member-orders")]
    public Task<PagedResultDto<OrderDto>> GetMemberOrdersAsync(GetOrderListDto input)
    {
        return memberAppService.GetMemberOrdersAsync(input);
    }

    //[HttpGet("member-order-stats/{id}")]
    //public Task<MemberCumulativeStatsDto> GetMemberCumulativeStatsAsync(Guid id)
    //{
    //    return memberAppService.GetMemberCumulativeStatsAsync(id);
    //}

    [HttpPut("{id}")]
    public Task<MemberDto> UpdateAsync(Guid id, UpdateMemberDto input)
    {
        return memberAppService.UpdateAsync(id, input);
    }
}
