using Kooco.Pikachu.Localization;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu;

/* Inherit your application services from this class.
 */
public abstract class PikachuAppService : ApplicationService
{
    protected PikachuAppService()
    {
        LocalizationResource = typeof(PikachuResource);
    }
}
