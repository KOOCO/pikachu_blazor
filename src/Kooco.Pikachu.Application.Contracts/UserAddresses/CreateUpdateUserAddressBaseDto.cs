using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserAddresses;

public class CreateUpdateUserAddressBaseDto
{
    [Required]
    [MaxLength(UserAddressConsts.MaxPostalCodeLength)]
    public virtual string PostalCode { get; set; }

    [Required]
    [MaxLength(UserAddressConsts.MaxCityLength)]
    public virtual string City { get; set; }

    [Required]
    [MaxLength(UserAddressConsts.MaxAddressLength)]
    public virtual string Address { get; set; }

    [Required]
    [MaxLength(UserAddressConsts.MaxRecipientNameLength)]
    public virtual string RecipientName { get; set; }

    [Required]
    [MaxLength(UserAddressConsts.MaxRecipientPhoneNumberLength)]
    public virtual string RecipientPhoneNumber { get; set; }

    [Required]
    public virtual bool IsDefault { get; set; }
}
