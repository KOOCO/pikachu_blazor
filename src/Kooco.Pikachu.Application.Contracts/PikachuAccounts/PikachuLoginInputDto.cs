using Kooco.Pikachu.EnumValues;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PikachuAccounts;

public class PikachuLoginInputDto
{
    [Required]
    public virtual LoginMethod? Method { get; set; }

    public string UserNameOrEmailAddress { get; set; }

    public string Password { get; set; }

    public virtual string? ThirdPartyToken { get; set; }
}