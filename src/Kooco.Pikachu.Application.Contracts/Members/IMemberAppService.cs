using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.UserCumulativeOrders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Members;

public interface IMemberAppService : IApplicationService
{
    Task<MemberDto> GetAsync(Guid id);
    Task<PagedResultDto<MemberDto>> GetListAsync(GetMemberListDto input);
    Task DeleteAsync(Guid id);
    Task<MemberDto> UpdateAsync(Guid id, UpdateMemberDto input);
    Task<UserAddressDto?> GetDefaultAddressAsync(Guid id);
    Task<PagedResultDto<OrderDto>> GetMemberOrderRecordsAsync(Guid id, GetMemberOrderRecordsDto input);
    Task<List<KeyValueDto>> GetGroupBuyLookupAsync();
    Task<PagedResultDto<MemberCreditRecordDto>> GetMemberCreditRecordAsync(Guid id, GetMemberCreditRecordListDto input);
    Task<UserCumulativeCreditDto> GetMemberCumulativeCreditsAsync(Guid id);
    Task<UserCumulativeOrderDto> GetMemberCumulativeOrdersAsync(Guid id);
    Task<UserCumulativeFinancialDto> GetMemberCumulativeFinancialsAsync(Guid id);
    Task<UserAddressDto> CreateMemberAddressAsync(Guid id, CreateUpdateMemberAddressDto input);
    Task<UserAddressDto> UpdateMemberAddressAsync(Guid id, Guid addressId, CreateUpdateMemberAddressDto input);
    Task<List<UserAddressDto>> GetMemberAddressListAsync(Guid id);
    Task SetBlacklistedAsync(Guid memberId, bool blacklisted);
    Task<MemberLoginResponseDto> LoginAsync(MemberLoginInputDto input);
    Task<MemberDto> RegisterAsync(CreateMemberDto input);
    Task<List<MemberDto>> GetAllAsync();
    Task<Guid?> GetCurrentUserIdAsync();
    Task<OrderDto> GetMemberOrderAsync(Guid orderId);
    Task<List<MemberOrderInfoDto>> GetMemberOrdersByGroupBuyAsync(Guid groupBuyId);
    Task<List<MemberOrderInfoDto>> GetMemberOrdersByGroupBuyAsync(Guid memberId, Guid groupBuyId);
}
