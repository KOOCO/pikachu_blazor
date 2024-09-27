using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PikachuAccounts;

public class PikachuLoginInputDto
{
    [Required]
    public string UserNameOrEmailAddress { get; set; }

    [Required]
    public string Password { get; set; }
}