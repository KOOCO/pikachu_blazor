using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Kooco.Pikachu.Blazor;

[Dependency(ReplaceServices = true)]
public class PikachuBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Pikachu";
}
