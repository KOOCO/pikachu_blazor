using Kooco.Pikachu.Constants;
using Kooco.Pikachu.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Volo.Abp.Modularity;

namespace Kooco.Pikachu;
public sealed class PikachuECPayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddRefitClient<IECPayEinvoice>().ConfigureHttpClient((provider, client) =>
        {
            var options = provider.GetService<IOptions<ECPayOptions>>()?.Value;
            if (options is not null)
            {
                client.BaseAddress = options.IsFormalArea ?
                    new Uri(ECPayConstants.Einvoice.FormalUrl) :
                    new Uri(ECPayConstants.Einvoice.TestUrl);

            }
        });
    }
}