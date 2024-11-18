using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.EnumValues;

public enum ReceivingTime
{
    Weekday9To13,
    Weekday14To18,
    Weekend9To13,
    Weekend14To18,
    UnableToSpecifyDuringPeakPeriods,
    PleaseContactUs,

    Before13PM,
    Between14To18PM,
    Inapplicable,
    UnableToSpecify
}
