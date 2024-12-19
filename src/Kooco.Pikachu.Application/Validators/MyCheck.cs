using Kooco.Pikachu.WebsiteManagement;
using System;
using Volo.Abp;

namespace Kooco.Pikachu.Validators;

public static class MyCheck
{
    public static void NotDefaultOrNull<TEnum>(object value, string paramName) where TEnum : struct, Enum
    {
        if (value == null || !Enum.IsDefined(typeof(TEnum), value))
        {
            throw new InvalidEnumValueException(paramName);
        }
    }

    public static void NotDefault<TEnum>(object value, string paramName) where TEnum : struct, Enum
    {
        if (value != null && !Enum.IsDefined(typeof(TEnum), value))
        {
            throw new InvalidEnumValueException(paramName);
        }
    }
}
