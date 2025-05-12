using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.ImageCompressors;

public class ImageCompressorService : ITransientDependency, IImageCompressorService
{
    private readonly ImageCompressorConfiguration _apiOptions;
    private readonly IRestClient _restClient;

    public ImageCompressorService(
        IOptions<ImageCompressorConfiguration> imageCompressorConfiguration
        )
    {
        _apiOptions = imageCompressorConfiguration.Value;
        _restClient = new RestClient(_apiOptions.ApiBaseUrl)
            .AddDefaultHeader("Authorization", $"Bearer {_apiOptions.PassKey}");
    }

    public async Task<CompressImageResponse> CompressAsync(byte[] bytes)
    {
        var base64 = Convert.ToBase64String(bytes);
        return await CompressAsync(base64);
    }

    public async Task<CompressImageResponse> CompressAsync(string base64)
    {
        var request = new RestRequest("compressimage", Method.Post);
        request.AddJsonBody(new { image = base64 });

        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
        {
            string errorMessage = "Compression Failed";

            try
            {
                var errorData = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content);
                if (errorData != null && errorData.TryGetValue("message", out var message) && !string.IsNullOrWhiteSpace(message))
                {
                    errorMessage += ": " + message;
                }
            }
            catch { }

            throw new UserFriendlyException(errorMessage);
        }

        var compressedResponse = JsonSerializer.Deserialize<CompressImageResponse>(response.Content)!;
        compressedResponse.CompressedBytes = Convert.FromBase64String(compressedResponse.CompressedImage);

        return compressedResponse;
    }
}
