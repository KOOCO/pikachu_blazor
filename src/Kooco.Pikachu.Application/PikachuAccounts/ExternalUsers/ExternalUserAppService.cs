using RestSharp;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kooco.Pikachu.PikachuAccounts.ExternalUsers;

public class ExternalUserAppService : PikachuAppService, IExternalUserAppService
{
    public async Task<FacebookUserDto?> GetFacebookUserDetailsAsync(string accessToken)
    {
        var client = new RestClient("https://graph.facebook.com/v10.0");
        var request = new RestRequest("me", Method.Get);
        request.AddParameter("fields", "id,name,email");
        request.AddParameter("access_token", accessToken);

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful && response.Content != null)
        {
            return JsonSerializer.Deserialize<FacebookUserDto>(response.Content);
        }

        return default;
    }

    public async Task<GoogleUserDto?> GetGoogleUserDetailsAsync(string accessToken)
    {
        var client = new RestClient("https://www.googleapis.com/oauth2/v3");
        var request = new RestRequest("userinfo", Method.Get);
        request.AddParameter("access_token", accessToken);

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful && response.Content != null)
        {
            return JsonSerializer.Deserialize<GoogleUserDto>(response.Content);
        }

        return default;
    }
}
