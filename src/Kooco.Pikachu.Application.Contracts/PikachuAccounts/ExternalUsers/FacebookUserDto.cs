using System.Text.Json.Serialization;

namespace Kooco.Pikachu.PikachuAccounts.ExternalUsers;

public class FacebookUserDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}
