using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kooco.Pikachu.PaymentGateways.LinePay;

public class LinePayConfiguration
{
    public string SelfUrl { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string PaymentApiPath { get; set; } = string.Empty;
    public string ConfirmPaymentApiPath { get; set; } = string.Empty;
    public string SuccessReturnCode { get; set; } = string.Empty;
}

public class ConfigureLinePayOptions(IConfiguration configuration) : IConfigureOptions<LinePayConfiguration>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(LinePayConfiguration options)
    {
        var section = _configuration.GetSection("PaymentGateway:LinePay");
        section.Bind(options);

        options.SelfUrl = _configuration["App:SelfUrl"]?.TrimEnd('/') ?? string.Empty;

        options.ApiBaseUrl = options.ApiBaseUrl?.TrimEnd('/') ?? string.Empty;
        options.PaymentApiPath = options.PaymentApiPath?.TrimEnd('/') ?? string.Empty;
        options.ConfirmPaymentApiPath = options.ConfirmPaymentApiPath?.TrimEnd('/') ?? string.Empty;
        options.SuccessReturnCode ??= string.Empty;
    }
}
