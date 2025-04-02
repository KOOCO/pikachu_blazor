using System.Globalization;

namespace Kooco.Pikachu.Extensions;
public static class RepositoryExtensions
{
    public static string ToFuzzyPattern(this string? searchTerm) => $"%{searchTerm}%";
    public static string FormatAmountWithThousandsSigns(this decimal value, string cultureName = "zh-TW")
    {
        return value.ToString("C0", new CultureInfo(cultureName));
    }
}