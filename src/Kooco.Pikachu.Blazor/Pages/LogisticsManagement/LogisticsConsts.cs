using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.LogisticsManagement
{
    public static class LogisticsConsts
    {
        public static List<string> LogisticsTypes = new() { "B2C", "C2C" };
        public static List<string> LogisticsSubTypes = new() { "711", "全家" };
        public static List<string> MainIslands = new()
        {
            "TaipeiCity",
            "NewTaipeiCity",
            "Keelung",
            "Hsinchu",
            "Taoyuan",
            "Yilan",
            "Taichung",
            "Miaoli",
            "Changhua",
            "Nantou",
            "Yunlin",
            "Kaohsiung",
            "Tainan",
            "ChiayiCity",
            "ChiayiCounty",
            "PingDong",
            "Hualien",
            "Taitung"
        };
        public static List<string> OuterIslands = new() { "Penghu", "Kinmen", "Mazu" };
    }
}
