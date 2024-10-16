namespace Kooco.Pikachu.PikachuAccounts;

public class GenericResponseDto(bool success, string? errorMessage = null)
{
    public bool Success { get; set; } = success;
    public string? ErrorMessage { get; set; } = errorMessage;
}