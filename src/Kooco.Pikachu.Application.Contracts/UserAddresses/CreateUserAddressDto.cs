using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserAddresses;

public class CreateUserAddressDto : CreateUpdateUserAddressBaseDto
{
    [Required]
    public virtual Guid? UserId { get; set; }
}