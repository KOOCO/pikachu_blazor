namespace Kooco.Pikachu.PikachuAccounts.ExternalUsers;

public class GoogleUserDto
{
    public string Sub { get; set; }  // Google's unique user ID
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public string Picture { get; set; }
}