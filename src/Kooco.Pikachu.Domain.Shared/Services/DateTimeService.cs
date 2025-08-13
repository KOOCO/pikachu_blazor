using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using System;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Services;

public class DateTimeService : ITransientDependency
{
    private readonly IStringLocalizer<PikachuResource> _localizer;

    public DateTimeService(IStringLocalizer<PikachuResource> localizer)
    {
        _localizer = localizer;
    }

    public string Humanize(DateTime dateTime, bool isUtc = true)
    {
        var (time, localizationKey) = dateTime.HumanizeTime(isUtc);
        return _localizer[localizationKey, time];
    }
}
