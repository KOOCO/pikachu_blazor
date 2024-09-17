using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.UserAddresses;

public class UserAddress : FullAuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public string? PostalCode { get; private set; }
    public string? City { get; private set; }
    public string? Street { get; private set; }
    public string? RecipientName { get; private set; }
    public string? RecipientPhoneNumber { get; private set; }
    public bool IsDefault { get; set; }

    public UserAddress(
        Guid id,
        Guid userId,
        string postalCode,
        string city,
        string street,
        string recipientName,
        string recipientPhoneNumber,
        bool isDefault
        ) : base(id)
    {
        UserId = userId;
        SetPostalCode(postalCode);
        SetCity(city);
        SetStreet(street);
        SetRecipientName(recipientName);
        SetRecipientPhoneNumber(recipientPhoneNumber);
        SetIsDefault(isDefault);
    }

    internal UserAddress ChangeRecipientPhoneNumber(string recipientPhoneNumber)
    {
        SetRecipientPhoneNumber(recipientPhoneNumber);
        return this;
    }

    private void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        RecipientPhoneNumber = Check.NotNullOrWhiteSpace(recipientPhoneNumber, nameof(recipientPhoneNumber), maxLength: UserAddressConsts.MaxRecipientPhoneNumberLength);
    }

    internal UserAddress ChangeRecipientName(string recipientName)
    {
        SetRecipientName(recipientName);
        return this;
    }

    private void SetRecipientName(string recipientName)
    {
        RecipientName = Check.NotNullOrWhiteSpace(recipientName, nameof(recipientName), maxLength: UserAddressConsts.MaxRecipientNameLength);
    }

    internal UserAddress ChangeStreet(string street)
    {
        SetStreet(street);
        return this;
    }

    private void SetStreet(string street)
    {
        Street = Check.NotNullOrWhiteSpace(street, nameof(street), maxLength: UserAddressConsts.MaxStreetLength);
    }

    internal UserAddress ChangeCity(string city)
    {
        SetCity(city);
        return this;
    }

    private void SetCity(string city)
    {
        City = Check.NotNullOrWhiteSpace(city, nameof(city), maxLength: UserAddressConsts.MaxCityLength);
    }

    internal UserAddress ChangePostalCode(string postalCode)
    {
        SetPostalCode(postalCode);
        return this;
    }

    private void SetPostalCode(string postalCode)
    {
        PostalCode = Check.NotNullOrWhiteSpace(postalCode, nameof(postalCode), maxLength: UserAddressConsts.MaxPostalCodeLength);
    }

    public UserAddress SetIsDefault(bool isDefault)
    {
        IsDefault = isDefault;
        return this;
    }
}
