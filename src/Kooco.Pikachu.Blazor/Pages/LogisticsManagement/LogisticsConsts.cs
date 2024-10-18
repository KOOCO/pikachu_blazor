using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.LogisticsManagement
{
    public static class LogisticsConsts
    {
        public static List<string> LogisticsTypes = new() { "B2C", "C2C" };
        public static List<string> LogisticsSubTypes = new() { "711", "全家" };
        public static List<string> MainIslands = new()
        {
            "台北市",
            "新北市",
            "基隆市",
            "新竹市",
            "桃園市",
            "宜蘭縣",
            "台中市",
            "苗栗縣",
            "彰化縣",
            "南投縣",
            "雲林縣",
            "高雄市",
            "台南市",
            "嘉義市",
            "嘉義縣",
            "屏東縣",
            "花蓮縣",
            "台東縣"
        };
        public static List<string> OuterIslands = new() { "澎湖縣", "金門縣", "馬祖縣" };
    }
}
