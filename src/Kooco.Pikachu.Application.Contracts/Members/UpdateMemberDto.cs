using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Members;

public class UpdateMemberDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime? Birthday { get; set; } = new DateTime(2000, 1, 1);

    [Required]
    public string City { get; set; }

    [Required]
    public string Address { get; set; }
}
