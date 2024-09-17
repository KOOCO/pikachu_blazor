using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.UserAddresses;

public class UserAddressManager(IUserAddressRepository userAddressRepository) : DomainService
{
    public async Task<UserAddress> CreateAsync(Guid userId, string postalCode, string city, string street, string recipientName,
        string recipientPhoneNumber, bool isDefault)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.NotNullOrWhiteSpace(postalCode, nameof(postalCode), maxLength: UserAddressConsts.MaxPostalCodeLength);
        Check.NotNullOrWhiteSpace(city, nameof(city), maxLength: UserAddressConsts.MaxCityLength);
        Check.NotNullOrWhiteSpace(street, nameof(street), maxLength: UserAddressConsts.MaxStreetLength);
        Check.NotNullOrWhiteSpace(recipientName, nameof(recipientName), maxLength: UserAddressConsts.MaxRecipientNameLength);
        Check.NotNullOrWhiteSpace(recipientPhoneNumber, nameof(recipientPhoneNumber), maxLength: UserAddressConsts.MaxRecipientPhoneNumberLength);

        var userAddress = new UserAddress(GuidGenerator.Create(), userId, postalCode, city, street,
            recipientName, recipientPhoneNumber, isDefault);

        await userAddressRepository.InsertAsync(userAddress);
        return userAddress;
    }

    public async Task<UserAddress> UpdateAsync(UserAddress userAddress, Guid userId, string postalCode, string city, string street,
        string recipientName, string recipientPhoneNumber, bool isDefault)
    {
        Check.NotNull(userAddress, nameof(UserAddress));
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.NotNullOrWhiteSpace(postalCode, nameof(postalCode), maxLength: UserAddressConsts.MaxPostalCodeLength);
        Check.NotNullOrWhiteSpace(city, nameof(city), maxLength: UserAddressConsts.MaxCityLength);
        Check.NotNullOrWhiteSpace(street, nameof(street), maxLength: UserAddressConsts.MaxStreetLength);
        Check.NotNullOrWhiteSpace(recipientName, nameof(recipientName), maxLength: UserAddressConsts.MaxRecipientNameLength);
        Check.NotNullOrWhiteSpace(recipientPhoneNumber, nameof(recipientPhoneNumber), maxLength: UserAddressConsts.MaxRecipientPhoneNumberLength);

        if (userId != Guid.Empty && userId != userAddress.UserId)
        {
            userAddress.UserId = userId;
        }

        userAddress.ChangePostalCode(postalCode);
        userAddress.ChangeCity(city);
        userAddress.ChangeStreet(street);
        userAddress.ChangeRecipientName(recipientName);
        userAddress.ChangeRecipientPhoneNumber(recipientPhoneNumber);
        userAddress.SetIsDefault(isDefault);

        await userAddressRepository.UpdateAsync(userAddress);
        return userAddress;
    }

    //public async Task<UserAddress> SetIsDefault(UserAddress userAddress, bool isDefault)
    //{
    //    if (isDefault)
    //    {
    //        var queryable = await userAddressRepository.GetQueryableAsync();
    //        //queryable.Where(x => x.IsDefault).Executeupdate
    //    }
    //}
}
