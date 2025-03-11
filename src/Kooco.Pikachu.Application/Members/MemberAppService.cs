using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.PikachuAccounts;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.UserCumulativeOrders;
using Kooco.Pikachu.Users;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Kooco.Pikachu.Members;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.Members.Default)]
public class MemberAppService(IMemberRepository memberRepository, IdentityUserManager identityUserManager,
    UserAddressManager userAddressManager, IOrderRepository orderRepository, IGroupBuyRepository groupBuyRepository,
    UserCumulativeCreditManager userCumulativeCreditManager, UserCumulativeOrderManager userCumulativeOrderManager,
    UserCumulativeFinancialManager userCumulativeFinancialManager,
    IUserShoppingCreditAppService userShoppingCreditAppService,
    IShoppingCreditEarnSettingAppService shoppingCreditEarnSettingAppService,
    IPikachuAccountAppService pikachuAccountAppService, IUserAddressRepository userAddressRepository, IUserCumulativeCreditAppService userCumulativeCreditAppService, IUserCumulativeCreditRepository userCumulativeCreditRepository) : PikachuAppService, IMemberAppService
{
    public async Task<MemberDto> GetAsync(Guid id)
    {
        var member = await memberRepository.GetAsync(id);
        return ObjectMapper.Map<IdentityUser, MemberDto>(member);
    }

    public async Task<PagedResultDto<MemberDto>> GetListAsync(GetMemberListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(IdentityUser.UserName);
        }

        var totalCount = await memberRepository.GetCountAsync(input.Filter);
        var items = await memberRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);
        return new PagedResultDto<MemberDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<MemberModel>, List<MemberDto>>(items)
        };
    }

    public async Task<List<MemberDto>> GetAllAsync()
    {
        var sorting = nameof(IdentityUser.UserName);
        var items = await memberRepository.GetListAsync(0, int.MaxValue, sorting, null);
        return ObjectMapper.Map<List<MemberModel>, List<MemberDto>>(items);
    }

    [Authorize(PikachuPermissions.Members.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var member = await memberRepository.GetAsync(id);
        await memberRepository.DeleteAsync(member);
    }

    [Authorize(PikachuPermissions.Members.Edit)]
    public async Task<MemberDto> UpdateAsync(Guid id, UpdateMemberDto input)
    {
        Check.NotNull(input, nameof(input));

        var member = await memberRepository.GetAsync(id);

        member.Name = input.Name;

        (await identityUserManager.SetEmailAsync(member, input.Email)).CheckErrors();
        (await identityUserManager.SetPhoneNumberAsync(member, input.PhoneNumber)).CheckErrors();

        member.SetBirthday(input.Birthday);
        member.SetMobileNumber(input.MobileNumber);
        member.SetGender(input.Gender);

        (await identityUserManager.UpdateAsync(member)).CheckErrors();

        if (input.DefaultAddressId.HasValue)
        {
            await userAddressManager.SetIsDefaultAsync(input.DefaultAddressId.Value, true);
        }

        return ObjectMapper.Map<IdentityUser, MemberDto>(member);
    }

    public async Task<UserAddressDto?> GetDefaultAddressAsync(Guid id)
    {
        var defaultAddress = await userAddressManager.GetDefaultAddressAsync(id);
        return ObjectMapper.Map<UserAddress?, UserAddressDto?>(defaultAddress);
    }

    public async Task<PagedResultDto<OrderDto>> GetMemberOrderRecordsAsync(Guid id, GetMemberOrderRecordsDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(Order.CreationTime) + " DESC";
        }

        var queryable = await orderRepository.GetQueryableAsync();

        queryable = queryable
                .Where(x => x.UserId == id)
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => x.OrderNo.Contains(input.Filter))
                .WhereIf(input.StartDate.HasValue, x => x.CreationTime.Date >= input.StartDate.Value.Date)
                .WhereIf(input.EndDate.HasValue, x => x.CreationTime.Date <= input.EndDate.Value)
                .WhereIf(input.GroupBuyId.HasValue, x => x.GroupBuyId == input.GroupBuyId)
                .WhereIf(input.ShippingStatus.HasValue, x => x.ShippingStatus == input.ShippingStatus)
                .WhereIf(input.DeliveryMethod.HasValue, x => x.DeliveryMethod == input.DeliveryMethod);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(queryable.OrderBy(input.Sorting).PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<OrderDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
        };
    }

    public async Task<PagedResultDto<MemberCreditRecordDto>> GetMemberCreditRecordAsync(Guid id, GetMemberCreditRecordListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(MemberCreditRecordModel.UsageTime) + " DESC";
        }

        Check.NotDefaultOrNull<Guid>(id, nameof(id));

        var totalCount = await memberRepository.GetMemberCreditRecordCountAsync(input.Filter, input.UsageTimeFrom, input.UsageTimeTo,
            input.ExpiryDateFrom, input.ExpiryDateTo, input.MinRemainingCredits, input.MaxRemainingCredits, input.MinAmount, input.MaxAmount, id);

        var items = await memberRepository.GetMemberCreditRecordListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter,
            input.UsageTimeFrom, input.UsageTimeTo, input.ExpiryDateFrom, input.ExpiryDateTo, input.MinRemainingCredits, input.MaxRemainingCredits,
            input.MinAmount, input.MaxAmount, id);

        return new PagedResultDto<MemberCreditRecordDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<MemberCreditRecordModel>, List<MemberCreditRecordDto>>(items)
        };
    }

    public async Task<UserCumulativeCreditDto> GetMemberCumulativeCreditsAsync(Guid id)
    {
        var userCumulativeCredit = await userCumulativeCreditManager.FirstOrDefaultByUserIdAsync(id);
        return ObjectMapper.Map<UserCumulativeCredit, UserCumulativeCreditDto>(userCumulativeCredit);
    }

    public async Task<UserCumulativeOrderDto> GetMemberCumulativeOrdersAsync(Guid id)
    {
        var userCumulativeOrder = await userCumulativeOrderManager.FirstOrDefaultByUserIdAsync(id);
        return ObjectMapper.Map<UserCumulativeOrder, UserCumulativeOrderDto>(userCumulativeOrder);
    }

    public async Task<UserCumulativeFinancialDto> GetMemberCumulativeFinancialsAsync(Guid id)
    {
        var userCumulativeFinancial = await userCumulativeFinancialManager.FirstOrDefaultByUserIdAsync(id);
        return ObjectMapper.Map<UserCumulativeFinancial, UserCumulativeFinancialDto>(userCumulativeFinancial);
    }

    public async Task<List<KeyValueDto>> GetGroupBuyLookupAsync()
    {
        var queryable = await groupBuyRepository.GetQueryableAsync();
        return [.. queryable.Select(x => new KeyValueDto { Id = x.Id, Name = x.GroupBuyName })];
    }

    public async Task<UserAddressDto> CreateMemberAddressAsync(Guid id, CreateUpdateMemberAddressDto input)
    {
        Check.NotDefaultOrNull<Guid>(id, nameof(id));
        Check.NotNull(input, nameof(input));

        var userAddress = await userAddressManager.CreateAsync(id, input.PostalCode, input.City, input.Address, input.RecipientName,
            input.RecipientPhoneNumber, input.IsDefault);

        return ObjectMapper.Map<UserAddress, UserAddressDto>(userAddress);
    }

    public async Task<UserAddressDto> UpdateMemberAddressAsync(Guid id, Guid addressId, CreateUpdateMemberAddressDto input)
    {
        Check.NotDefaultOrNull<Guid>(id, nameof(id));
        Check.NotNull(input, nameof(input));

        var userAddress = await userAddressManager.GetByIdAsync(addressId);
        await userAddressManager.UpdateAsync(userAddress, id, input.PostalCode, input.City, input.Address, input.RecipientName,
            input.RecipientPhoneNumber, input.IsDefault);

        return ObjectMapper.Map<UserAddress, UserAddressDto>(userAddress);
    }

    public async Task<List<UserAddressDto>> GetMemberAddressListAsync(Guid id)
    {
        var queryable = await userAddressRepository.GetFilteredQueryableAsync(userId: id);
        return ObjectMapper.Map<List<UserAddress>, List<UserAddressDto>>([.. queryable]);
    }

    public async Task<OrderDto> GetMemberOrderAsync(Guid orderId)
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new EntityNotFoundException(nameof(IdentityUser));
        }

        var order = await orderRepository.GetMemberOrderAsync(orderId, CurrentUser.Id.Value);

        if (order == null || order.UserId != CurrentUser.Id)
        {
            throw new EntityNotFoundException(typeof(Order), orderId);
        }

        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    public async Task<List<MemberOrderInfoDto>> GetMemberOrdersByGroupBuyAsync(Guid groupBuyId)
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new EntityNotFoundException(nameof(IdentityUser));
        }

        var orders = await orderRepository.GetMemberOrdersByGroupBuyAsync(CurrentUser.Id.Value, groupBuyId);
        return ObjectMapper.Map<List<MemberOrderInfoModel>, List<MemberOrderInfoDto>>(orders);
    }

    [AllowAnonymous]
    public async Task<MemberLoginResponseDto> LoginAsync(MemberLoginInputDto input)
    {
        Check.NotNull(input, nameof(input));

        var loginResult = await pikachuAccountAppService.LoginAsync(input);
        if (loginResult is null)
        {
            return new MemberLoginResponseDto(false);
        }
        return ObjectMapper.Map<PikachuLoginResponseDto, MemberLoginResponseDto>(loginResult);
    }

    [AllowAnonymous]
    public async Task<MemberDto> RegisterAsync(CreateMemberDto input)
    {
        Check.NotNull(input, nameof(input));

        input.Role = MemberConsts.Role;
        var identityUser = await pikachuAccountAppService.RegisterAsync(input);
        var shoppingCredit = await shoppingCreditEarnSettingAppService.GetFirstAsync();
        if (shoppingCredit.RegistrationBonusEnabled)
        {
            await userShoppingCreditAppService.RecordShoppingCreditAsync(new RecordUserShoppingCreditDto
            {
                UserId = identityUser.Id,
                Amount = shoppingCredit.RegistrationEarnedPoints,
                IsActive = shoppingCredit.RegistrationBonusEnabled,
                ExpirationDate = shoppingCredit.RegistrationUsagePeriodType == "NoExpiry" ? null : DateTime.Today.AddDays(shoppingCredit.RegistrationValidDays),
                TransactionDescription = "獲得註冊禮金",
                ShoppingCreditType = UserShoppingCreditType.Grant
            });
            var userCumulativeCredit = await userCumulativeCreditRepository.FirstOrDefaultAsync(x => x.UserId == identityUser.Id);
            if (userCumulativeCredit is null)
            {
                await userCumulativeCreditAppService.CreateMemberRegisterAsync(new CreateUserCumulativeCreditDto { TotalAmount = shoppingCredit.RegistrationEarnedPoints, TotalDeductions = 0, TotalRefunds = 0, UserId = identityUser.Id });
            }
            else
            {
                userCumulativeCredit.ChangeTotalAmount((int)(userCumulativeCredit.TotalAmount + shoppingCredit.RegistrationEarnedPoints));
                await userCumulativeCreditRepository.UpdateAsync(userCumulativeCredit);
            }

        }
        return ObjectMapper.Map<IdentityUserDto, MemberDto>(identityUser);
    }
}
