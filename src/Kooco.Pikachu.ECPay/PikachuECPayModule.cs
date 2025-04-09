using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Kooco.Pikachu;

[DependsOn(typeof(AbpHttpClientModule))]
public sealed class PikachuECPayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClient(nameof(ECPayConstants.Einvoice), client =>
        {
            client.BaseAddress = new(ECPayConstants.Einvoice.FormalUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new(ECPayConstants.MediaType));
        });
    }
}