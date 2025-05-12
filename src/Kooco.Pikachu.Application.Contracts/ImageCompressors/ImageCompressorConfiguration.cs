using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kooco.Pikachu.ImageCompressors;

public class ImageCompressorConfiguration
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string PassKey { get; set; } = string.Empty;
}

public class ConfigureImageCompressorOptions(IConfiguration configuration) : IConfigureOptions<ImageCompressorConfiguration>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(ImageCompressorConfiguration options)
    {
        var section = _configuration.GetSection("ImageCompressor");
        section.Bind(options);

        options.ApiBaseUrl = options.ApiBaseUrl?.TrimEnd('/') ?? string.Empty;
    }
}