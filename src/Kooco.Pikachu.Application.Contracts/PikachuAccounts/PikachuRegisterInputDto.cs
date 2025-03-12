using Kooco.Pikachu.EnumValues;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kooco.Pikachu.PikachuAccounts;

public class PikachuRegisterInputDto
{
    [Required]
    public virtual LoginMethod? Method { get; set; }

    [EmailAddress]
    public virtual string? Email { get; set; }

    public virtual string? UserName { get; set; }

    public virtual string? Name { get; set; }

    public virtual string? PhoneNumber { get; set; }

    public virtual DateTime? Birthday { get; set; }

    public virtual string? MobileNumber { get; set; }

    public virtual string? Gender { get; set; }

    [DataType(DataType.Password)]
    public virtual string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public virtual string? ConfirmPassword { get; set; }

    public virtual string? ThirdPartyToken { get; set; }

    [JsonIgnore]
    public virtual string? Role { get; set; }

    [JsonIgnore]
    public virtual string? ExternalId { get; set; }
    [JsonIgnore]
    public virtual bool isCallFromTest { get; set; }
}