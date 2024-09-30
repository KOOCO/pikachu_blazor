using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.UserAddresses;

public interface IUserAddressRepository : IRepository<UserAddress, Guid>
{
    Task RemoveDefaultUserAddressesAsync(Guid userId);
    Task<long> GetCountAsync(string? filter, Guid? userId, string? postalCode,
        string? city, string? address, string? recipientName, string? recipientPhoneNumber, bool? isDefault);
    Task<List<UserAddress>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter,
        Guid? userId, string? postalCode, string? city, string? address, string? recipientName,
        string? recipientPhoneNumber, bool? isDefault);

    Task<IQueryable<UserAddress>> GetFilteredQueryableAsync(
            string? filter = null,
            Guid? userId = null,
            string? postalCode = null,
            string? city = null,
            string? address = null,
            string? recipientName = null,
            string? recipientPhoneNumber = null,
            bool? isDefault = null
            );
    Task<UserAddress?> GetDefaultAddressAsync(Guid userId);
}
