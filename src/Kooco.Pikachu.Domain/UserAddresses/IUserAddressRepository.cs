using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.UserAddresses;

public interface IUserAddressRepository : IRepository<UserAddress, Guid>
{
    Task RemoveDefaultUserAddressesAsync(Guid userId);
    Task<long> GetCountAsync(string? filter, Guid? userId, string? postalCode,
        string? city, string? street, string? recipientName, string? recipientPhoneNumber, bool? isDefault);
    Task<List<UserAddress>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        Guid? userId, string? postalCode, string? city, string? street, string? recipientName,
        string? recipientPhoneNumber, bool? isDefault);
    Task<UserAddress?> GetDefaultAddressAsync(Guid userId);
}
