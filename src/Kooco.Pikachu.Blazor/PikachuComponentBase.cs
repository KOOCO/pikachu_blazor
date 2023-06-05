using Kooco.Pikachu.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Kooco.Pikachu.Blazor;

public abstract class PikachuComponentBase : AbpComponentBase
{
    protected PikachuComponentBase()
    {
        LocalizationResource = typeof(PikachuResource);
    }
}
