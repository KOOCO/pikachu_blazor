using System;
using System.ComponentModel;
using System.Reflection;

namespace Kooco.Pikachu.Blazor.Helpers;
public static class EnumHelper
{
    public static string GetDescription(Enum value)
    {
        return value.GetType()
                    .GetField(value.ToString())
                    ?.GetCustomAttribute<DescriptionAttribute>()?
                    .Description ?? value.ToString();
    }
}