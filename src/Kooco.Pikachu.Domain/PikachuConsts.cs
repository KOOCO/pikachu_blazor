using System;

namespace Kooco.Pikachu;
public static class PikachuConsts
{
    public const string DbTablePrefix = "App";
    public const string DbSchema = null;

    public static string ToDatabaseName(this Type type)
    {
        return $"{DbTablePrefix}{GetPluralForm(type.Name)}";
    }
    static string GetPluralForm(string word)
    {
        if (word.EndsWith('y') && word.Length > 1 && !IsVowel(word[^2]))
        {
            return string.Concat(word.AsSpan(0, word.Length - 1), "ies");
        }
        return word + "s";
    }
    static bool IsVowel(char c) => "aeiouAEIOU".Contains(c, StringComparison.Ordinal);
}