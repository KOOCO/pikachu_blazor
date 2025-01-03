using Asp.Versioning;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PikachuAccounts;
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
public class MemberController(IMemberAppService memberAppService, IPikachuAccountAppService pikachuAccountAppService) : PikachuController, IMemberAppService
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

    [HttpGet("{id}/order-records")]
    public Task<PagedResultDto<OrderDto>> GetMemberOrderRecordsAsync(Guid id, [FromQuery] GetMemberOrderRecordsDto input)
    {
        return memberAppService.GetMemberOrderRecordsAsync(id, input);
    }

    [HttpPut("{id}")]
    public Task<MemberDto> UpdateAsync(Guid id, UpdateMemberDto input)
    {
        return memberAppService.UpdateAsync(id, input);
    }

    [HttpPost("{id}/addresses")]
    public Task<UserAddressDto> CreateMemberAddressAsync(Guid id, CreateUpdateMemberAddressDto input)
    {
        return memberAppService.CreateMemberAddressAsync(id, input);
    }

    [HttpPut("{id}/addresses/{addressId}")]
    public Task<UserAddressDto> UpdateMemberAddressAsync(Guid id, Guid addressId, CreateUpdateMemberAddressDto input)
    {
        return memberAppService.UpdateMemberAddressAsync(id, addressId, input);
    }

    [HttpGet("{id}/addresses/default")]
    public Task<UserAddressDto?> GetDefaultAddressAsync(Guid id)
    {
        return memberAppService.GetDefaultAddressAsync(id);
    }

    [HttpGet("{id}/addresses/list")]
    public Task<List<UserAddressDto>> GetMemberAddressListAsync(Guid id)
    {
        return memberAppService.GetMemberAddressListAsync(id);
    }

    [HttpPost("login")]
    public Task<MemberLoginResponseDto> LoginAsync(MemberLoginInputDto input)
    {
        return memberAppService.LoginAsync(input);
    }

    [HttpPost("register")]
    public Task<MemberDto> RegisterAsync(CreateMemberDto input)
    {
        return memberAppService.RegisterAsync(input);
    }

    [HttpPost("send-email-verification-code/{email}")]
    public Task<GenericResponseDto> SendEmailVerificationCodeAsync(string email)
    {
        return pikachuAccountAppService.SendEmailVerificationCodeAsync(email);
    }

    [HttpPost("verify-email-code/{email}/{code}")]
    public Task<VerifyCodeResponseDto> VerifyEmailCodeAsync(string email, string code)
    {
        return pikachuAccountAppService.VerifyEmailCodeAsync(email, code);
    }

    [HttpPost("send-password-reset-code/{email}")]
    public Task<GenericResponseDto> SendPasswordResetCodeAsync(string email)
    {
        return pikachuAccountAppService.SendPasswordResetCodeAsync(email);
    }

    [HttpPost("verify-password-reset-code/{email}/{code}")]
    public Task<VerifyCodeResponseDto> VerifyPasswordResetCodeAsync(string email, string code)
    {
        return pikachuAccountAppService.VerifyPasswordResetCodeAsync(email, code);
    }

    [HttpPost("reset-password")]
    public Task<GenericResponseDto> ResetPasswordAsync(PikachuResetPasswordDto input)
    {
        return pikachuAccountAppService.ResetPasswordAsync(input);
    }

    [HttpPost("find-by-token/{method}")]
    public Task<GenericResponseDto> FindByTokenAsync(LoginMethod method, [FromBody] string thirdPartyToken)
    {
        return pikachuAccountAppService.FindByTokenAsync(method, thirdPartyToken);
    }

    [HttpGet("get-all")]
    public Task<List<MemberDto>> GetAllAsync()
    {
        return memberAppService.GetAllAsync();
    }

    [HttpGet("order/{orderId}")]
    public Task<OrderDto> GetMemberOrderAsync(Guid orderId)
    {
        return memberAppService.GetMemberOrderAsync(orderId);
    }

    [HttpGet("order/list/{groupBuyId}")]
    public Task<List<MemberOrderInfoDto>> GetMemberOrdersByGroupBuyAsync(Guid groupBuyId)
    {
        return memberAppService.GetMemberOrdersByGroupBuyAsync(groupBuyId);
    }
}
