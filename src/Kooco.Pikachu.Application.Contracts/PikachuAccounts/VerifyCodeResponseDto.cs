namespace Kooco.Pikachu.PikachuAccounts;

public class VerifyCodeResponseDto
{
    public bool Verified { get; set; }
    public string? Email { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ResetToken { get; set; }

    public VerifyCodeResponseDto(bool emailConfirmed, string? email, string? errorMessage = null)
    {
        Verified = emailConfirmed;
        Email = email;
        ErrorMessage = errorMessage;
    }
}