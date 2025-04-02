using Humanizer;
using System;

namespace Kooco.Pikachu;
public static class PikachuConsts
{
    public const string DbTablePrefix = "App";
    public const string DbSchema = null;

    public static string ToDatabaseName(this Type type)
    {
        return $"{DbTablePrefix}{type.Name.Pluralize()}";
    }
}