namespace Kooco.Pikachu.PikachuAccounts;

public class PikachuLoginResponseDto(bool success)
{
    public bool Success { get; set; } = success;
    public string? AccessToken { get; set; }
    public string? ErrorDescription { get; set; }
}