using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.PikachuAccounts;

[RemoteService(IsEnabled = false)]
public class PikachuAccountAppService(IConfiguration configuration) : PikachuAppService, IPikachuAccountAppService
{
    public async Task<PikachuLoginResponseDto> LoginAsync(PikachuLoginInputDto input)
    {
        var selfUrl = configuration["App:SelfUrl"] ?? "";

        var options = new RestClientOptions(selfUrl);
        var restClient = new RestClient(options);
        var request = new RestRequest("/connect/token", Method.Post);

        request.AddHeader("Content-Type", ContentType.FormUrlEncoded);

        var param = new Dictionary<string, object>
        {
            { "username", input.UserNameOrEmailAddress },
            { "password", input.Password },
            { "grant_type", "password" },
            { "client_id", "Pikachu_App" },
            { "client_secret", "" },
            { "scope", "Pikachu" },
        };

        foreach (var kvp in param)
        {
            request.AddParameter(kvp.Key, kvp.Value, ParameterType.GetOrPost);
        }

        var response = await restClient.ExecuteAsync(request);

        using JsonDocument doc = JsonDocument.Parse(response.Content ?? "");
        JsonElement root = doc.RootElement;

        var result = new PikachuLoginResponseDto(response.IsSuccessStatusCode);
        if (result.Success)
        {
            root.TryGetProperty("access_token", out JsonElement accessTokenElement);

            if (accessTokenElement.ValueKind == JsonValueKind.String)
            {
                result.AccessToken = accessTokenElement.GetString();
            }
        }
        root.TryGetProperty("error_description", out JsonElement errorDescriptionElement);
        if (errorDescriptionElement.ValueKind == JsonValueKind.String)
        {
            result.ErrorDescription = errorDescriptionElement.GetString();
        }
        return result;
    }
}
