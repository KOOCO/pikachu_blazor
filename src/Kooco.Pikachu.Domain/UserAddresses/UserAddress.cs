using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.UserAddresses;

public class UserAddress : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid UserId { get; set; }
    public string PostalCode { get; private set; }
    public string City { get; private set; }
    public string Address { get; private set; }
    public string RecipientName { get; private set; }
    public string RecipientPhoneNumber { get; private set; }
    public bool IsDefault { get; private set; }
    public Guid? TenantId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }

    public UserAddress(
        Guid id,
        Guid userId,
        string postalCode,
        string city,
        string address,
        string recipientName,
        string recipientPhoneNumber,
        bool isDefault
        ) : base(id)
    {
        UserId = userId;
        SetPostalCode(postalCode);
        SetCity(city);
        SetAddress(address);
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

    internal UserAddress ChangeAddress(string address)
    {
        SetAddress(address);
        return this;
    }

    private void SetAddress(string address)
    {
        Address = Check.NotNullOrWhiteSpace(address, nameof(address), maxLength: UserAddressConsts.MaxAddressLength);
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
