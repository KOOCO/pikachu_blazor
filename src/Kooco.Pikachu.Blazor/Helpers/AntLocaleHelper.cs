using AntDesign;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kooco.Pikachu.Blazor.Helpers;

public class AntLocaleHelper
{
    public static DatePickerLocale GetLocale()
    {
        return CultureInfo.CurrentUICulture.Name switch
        {
            "zh-Hant" => new DatePickerLocale
            {
                DateLocale = new DateLocale
                {
                    YearFormat = "yyyy年",
                    MonthFormat = "M月",
                    DateSelect = "选择日期",
                    WeekSelect = "选择周",
                    MonthSelect = "选择月份",
                    YearSelect = "选择年份",
                    QuarterSelect = "选择季度",
                    Today = "今天",
                    ShortWeekDays = ["日", "一", "二", "三", "四", "五", "六"],
                    RangePlaceholder = ["開始日期", "結束日期"]
                    
                },
               
                
            },
            _ => new DatePickerLocale()
        };
    }

    public static string GetFormat()
    {
        return CultureInfo.CurrentUICulture.Name == "zh-Hant" ? "yyyy年MM月dd日" : "yyyy-MM-dd";
    }
}

public class AntHelper
{
    public static string FormatAmount(int? value)
    {
        return "$ " + value?.ToString("n0");
    }

    public static string ParseAmount(string value)
    {
        return Regex.Replace(value, @"\$\s?|(,*)", "");
    }
}
