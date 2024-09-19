using System;

namespace Kooco.Pikachu.UserAddresses;

public class UserAddressDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string RecipientName { get; set; }
    public string RecipientPhoneNumber { get; set; }
    public bool IsDefault { get; set; }
}