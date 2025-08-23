using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Kooco;

[DependsOn(typeof(AbpHttpClientModule))]
public sealed class ECPayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClient(nameof(ECPayConstants.Einvoice), client =>
        {
            client.BaseAddress = new(ECPayConstants.Einvoice.FormalUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new(HttpContentType.ApplicationJson));
        });

        context.Services.AddTransient<IConfigureOptions<EcPayHttpOptions>, ConfigureEcPayHttpOptions>();
    }
}