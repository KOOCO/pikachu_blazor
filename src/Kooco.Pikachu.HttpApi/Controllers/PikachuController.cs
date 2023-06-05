using Kooco.Pikachu.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class PikachuController : AbpControllerBase
{
    protected PikachuController()
    {
        LocalizationResource = typeof(PikachuResource);
    }
}
