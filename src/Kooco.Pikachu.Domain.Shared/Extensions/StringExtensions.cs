using System;

namespace Kooco.Pikachu.Extensions;

public static class StringExtensions
{
    public const string Today = "Today";
    public const string Week = "Week";
    public const string Month = "Month";

    public const string DefaultQuillHtml = "<p><br></p>";

    public static (DateTime, DateTime) FindFilterDateRange(this string dateRange)
    {
        DateTime minTime;
        DateTime maxTime;

        switch (dateRange)
        {
            case Today:
                minTime = DateTime.Today;
                maxTime = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                break;

            case Week:
                int daysToSubtract = (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysToSubtract < 0) daysToSubtract += 7;
                minTime = DateTime.Today.AddDays(-daysToSubtract);

                int daysToAdd = (int)DayOfWeek.Sunday - (int)DateTime.Today.DayOfWeek;
                if (daysToAdd < 0) daysToAdd += 7;
                maxTime = DateTime.Today.AddDays(daysToAdd).AddDays(1).AddMilliseconds(-1);
                break;

            case Month:
                minTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                maxTime = minTime.AddMonths(1).AddMilliseconds(-1);
                break;

            default:
                return (default, default);
        }

        return (minTime, maxTime);
    }

    public static bool IsEmptyOrValidUrl(this string? url)
    {
        if (url.IsNullOrWhiteSpace()) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public static bool IsEmptyOrDefaultQuillHtml(this string html)
    {
        if (html.IsNullOrWhiteSpace() || html.ToLower() == DefaultQuillHtml)
        {
            return true;
        }
        return false;
    }

    public static string ToMoneyString(this decimal number, string? format = null, string? currency = null)
    {
        currency ??= "$";
        format ??= "N2";
        return currency + number.ToString(format);
    }
    
    public static string ToPercentageString(this decimal number, string? format = null)
    {
        format ??= "N2";
        return number.ToString(format) + "%";
    }

    public static string ToNumberString(this int number, string? format = null)
    {
        format ??= "N0";
        return number.ToString(format);
    }
}
