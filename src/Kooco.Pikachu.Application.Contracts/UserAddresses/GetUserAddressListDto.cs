using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserAddresses;

public class GetUserAddressListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? UserId { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhoneNumber { get; set; }
    public bool IsDefault { get; set; }
}