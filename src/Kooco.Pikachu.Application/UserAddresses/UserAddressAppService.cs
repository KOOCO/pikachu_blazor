using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserAddresses;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.UserAddresses.Default)]
public class UserAddressAppService(UserAddressManager userAddressManager, IUserAddressRepository userAddressRepository) : PikachuAppService, IUserAddressAppService
{
    [Authorize(PikachuPermissions.UserAddresses.Create)]
    public async Task<UserAddressDto> CreateAsync(CreateUserAddressDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userAddress = await userAddressManager.CreateAsync(input.UserId.Value, input.PostalCode, input.City,
            input.Street, input.RecipientName, input.RecipientPhoneNumber, input.IsDefault);
        return ObjectMapper.Map<UserAddress, UserAddressDto>(userAddress);
    }

    [Authorize(PikachuPermissions.UserAddresses.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var userAddress = await userAddressRepository.GetAsync(id);
        await userAddressRepository.DeleteAsync(userAddress);
    }

    public async Task<UserAddressDto> GetAsync(Guid id)
    {
        var userAddress = await userAddressRepository.GetAsync(id);
        return ObjectMapper.Map<UserAddress, UserAddressDto>(userAddress);
    }

    public async Task<PagedResultDto<UserAddressDto>> GetListAsync(GetUserAddressListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(UserAddress.PostalCode);
        }

        var totalCount = await userAddressRepository.GetCountAsync(input.Filter, input.UserId, input.PostalCode,
            input.City, input.Street, input.RecipientName, input.RecipientPhoneNumber, input.IsDefault);

        var items = await userAddressRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter,
            input.UserId, input.PostalCode, input.City, input.Street, input.RecipientName, input.RecipientPhoneNumber, input.IsDefault);

        return new PagedResultDto<UserAddressDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<UserAddress>, List<UserAddressDto>>(items)
        };
    }

    [Authorize(PikachuPermissions.UserAddresses.SetIsDefault)]
    public async Task<UserAddressDto> SetIsDefaultAsync(Guid id, bool isDefault)
    {
        var userAddress = await userAddressRepository.GetAsync(id);
        await userAddressManager.SetIsDefaultAsync(userAddress, isDefault);
        return ObjectMapper.Map<UserAddress, UserAddressDto>(userAddress);
    }

    [Authorize(PikachuPermissions.UserAddresses.Edit)]
    public async Task<UserAddressDto> UpdateAsync(Guid id, UpdateUserAddressDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.UserId, nameof(input.UserId));

        var userAddress = await userAddressRepository.GetAsync(id);

        await userAddressManager.UpdateAsync(userAddress, input.UserId.Value, input.PostalCode, input.City,
            input.Street, input.RecipientName, input.RecipientPhoneNumber, input.IsDefault);
        return ObjectMapper.Map<UserAddress, UserAddressDto>(userAddress);
    }
}
