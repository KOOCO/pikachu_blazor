using System.ComponentModel.DataAnnotations;
using System;

namespace Kooco.Pikachu.UserAddresses
{
    public class UpdateUserAddressDto
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        [MaxLength(UserAddressConsts.MaxPostalCodeLength)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(UserAddressConsts.MaxCityLength)]
        public string City { get; set; }

        [Required]
        [MaxLength(UserAddressConsts.MaxStreetLength)]
        public string Street { get; set; }

        [Required]
        [MaxLength(UserAddressConsts.MaxRecipientNameLength)]
        public string RecipientName { get; set; }

        [Required]
        [MaxLength(UserAddressConsts.MaxRecipientPhoneNumberLength)]
        public string RecipientPhoneNumber { get; set; }

        [Required]
        public bool IsDefault { get; set; }
    }
}