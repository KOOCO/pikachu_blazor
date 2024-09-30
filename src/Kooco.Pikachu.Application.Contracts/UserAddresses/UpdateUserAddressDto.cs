using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserAddresses;

public class UpdateUserAddressDto : CreateUpdateUserAddressBaseDto
{
    [Required]
    public Guid? UserId { get; set; }
}