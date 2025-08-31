using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooco.Pikachu.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? source)
    {
        return source ?? [];
    }

    public static List<T> OrEmptyListIfNull<T>(this List<T>? source)
    {
        return source ?? [];
    }
}
