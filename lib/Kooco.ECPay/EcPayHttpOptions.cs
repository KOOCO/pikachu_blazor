using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Kooco;

public class EcPayHttpOptions
{
    public string MerchantID { get; set; } = string.Empty;
    public string HashKey { get; set; } = string.Empty;
    public string HashIV { get; set; } = string.Empty;
    public string QueryMediaFileUrl { get; set; } = string.Empty;
    public string CodQueryTradeInfoUrl { get; set; } = string.Empty;
}

public class ConfigureEcPayHttpOptions : IConfigureOptions<EcPayHttpOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureEcPayHttpOptions(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Configure(EcPayHttpOptions options)
    {
        _configuration.GetSection("EcPay").Bind(options);

        options.QueryMediaFileUrl = options.QueryMediaFileUrl.TrimEnd('/');
        options.CodQueryTradeInfoUrl = options.CodQueryTradeInfoUrl.TrimEnd('/');
    }
}