using Kooco.Pikachu.TierManagement;
using System;

namespace Kooco.Pikachu;

public class TaipeiTime
{
    public static TimeZoneInfo TaipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

    public static DateTime Today()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TaipeiTimeZone).Date;
    }

    public static DateTime Now()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TaipeiTimeZone);
    }

    public static DateTime Utc(DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, TaipeiTimeZone);
    }

    public static DateTime VipTierNextRun(VipTierResetFrequency resetFrequency)
    {
        var localNow = Now();
        var localNextRun = localNow.Date.AddMonths((int)resetFrequency);
        return localNextRun;
    }
}
