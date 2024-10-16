using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.PikachuAccounts;

public class PikachuResetPasswordDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string ResetToken { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}