using System.Text.Json.Serialization;

namespace Kooco.Pikachu.PikachuAccounts.ExternalUsers;

public class GoogleUserDto
{
    [JsonPropertyName("sub")]
    public string Sub { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; }

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("picture")]
    public string Picture { get; set; }
}