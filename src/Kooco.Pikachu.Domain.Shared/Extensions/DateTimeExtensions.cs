using System;

namespace Kooco.Pikachu.Extensions;

public static class DateTimeExtensions
{
    public static (int Time, string LocalizationKey) HumanizeTime(this DateTime dateTime, bool isUtc = true)
    {
        var now = isUtc ? DateTime.UtcNow : DateTime.Now;
        var timeSpan = now - dateTime;
        if (timeSpan.TotalSeconds < 10)
        {
            return ((int)timeSpan.TotalSeconds, RelativeTimeKeys.JustNow);
        }
        else if (timeSpan.TotalSeconds < 60)
        {
            return ((int)timeSpan.TotalSeconds, RelativeTimeKeys.SecondsAgo);
        }
        else if (timeSpan.TotalMinutes < 60)
        {
            return ((int)timeSpan.TotalMinutes, RelativeTimeKeys.MinutesAgo);
        }
        else if (timeSpan.TotalHours < 24)
        {
            return ((int)timeSpan.TotalHours, RelativeTimeKeys.HoursAgo);
        }
        else if (timeSpan.TotalDays < 30)
        {
            return ((int)timeSpan.TotalDays, RelativeTimeKeys.DaysAgo);
        }
        else if (timeSpan.TotalDays < 365)
        {
            return ((int)(timeSpan.TotalDays / 30), RelativeTimeKeys.MonthsAgo);
        }
        else
        {
            return ((int)(timeSpan.TotalDays / 365), RelativeTimeKeys.YearsAgo);
        }
    }
}

public class RelativeTimeKeys
{
    public const string Prefix = "RelativeTime:";
    public const string JustNow = Prefix + "JustNow";
    public const string SecondsAgo = Prefix + "SecondsAgo";
    public const string MinutesAgo = Prefix + "MinutesAgo";
    public const string HoursAgo = Prefix + "HoursAgo";
    public const string DaysAgo = Prefix + "DaysAgo";
    public const string MonthsAgo = Prefix + "MonthsAgo";
    public const string YearsAgo = Prefix + "YearsAgo";
}